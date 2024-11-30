using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace covercy_task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiKeyController : ControllerBase
    {
        private readonly ApiKeyContext _context;
        private readonly ApiKeyService _apiKeyService;


        private bool AuthenticateRequest()
        {
            //TODO implement this
            return true;
        }

        public ApiKeyController(ApiKeyContext context, ApiKeyService apiKeyService)
        {
            _context = context;
            _apiKeyService = apiKeyService;
        }

        // 1. Create API Key
        [HttpPost]
        public async Task<ActionResult<ApiKeyResponse>> CreateApiKey([FromBody] CreateApiKeyRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId))
                return BadRequest("User ID is required");

            if (request.Permissions == null || request.Permissions.Count == 0)
                return BadRequest("User permissions is required");


            var apiKey = new ApiKey
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Key = _apiKeyService.GenerateApiKey(),
                Permissions = request.Permissions,
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            _context.ApiKeys.Add(apiKey);
            await _context.SaveChangesAsync();

            return Ok(new ApiKeyResponse
            {
                Id = apiKey.Id,
                Key = apiKey.Key,
                Permissions = apiKey.Permissions,
                CreatedAt = apiKey.CreatedAt,
                IsRevoked = apiKey.IsRevoked
            });
        }

        // 2. Authenticate with API Key
        [HttpPost("authenticate")]
        public async Task<ActionResult> AuthenticateWithApiKey([FromBody] string apiKeyValue)
        {
            var apiKey = await _context.ApiKeys
                .FirstOrDefaultAsync(a => a.Key == apiKeyValue && !a.IsRevoked);

            if (apiKey == null)
                return Unauthorized("Invalid or revoked API key");

          
            apiKey.LastUsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

          
            var token = _apiKeyService.GenerateJwtToken(apiKey);
            return Ok(new
            {
                Token = token,
                ApiKey = apiKey
            });
        }

        // 3. Revoke API Key
        [HttpDelete("{id}")]
        public async Task<ActionResult> RevokeApiKey(Guid id)
        {
            if (!AuthenticateRequest())
                return Unauthorized();

            var apiKey = await _context.ApiKeys
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apiKey == null)
                return NotFound();

            apiKey.IsRevoked = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 4. Get User's Tokens
        [HttpGet("{userId}")]
        public async Task<ActionResult<List<ApiKeyResponse>>> GetUserTokens(string userId)
        {


            if (!AuthenticateRequest() && userId == null)
                return Unauthorized();

            var apiKeys = await _context.ApiKeys
                .Where(a => a.UserId == userId)
                .Select(a => new ApiKeyResponse
                {
                    Id = a.Id,
                    Key = a.Key,
                    Permissions = a.Permissions,
                    CreatedAt = a.CreatedAt,
                    LastUsedAt = a.LastUsedAt,
                    IsRevoked = a.IsRevoked
                })
                .ToListAsync();

            return Ok(apiKeys);
        }
    }
}
