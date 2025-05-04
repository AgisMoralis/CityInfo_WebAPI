using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        public class AuthenticationRequestBody
        {
            public string? Username { get; set; }
            public string? Password { get; set; }
        }

        public class CityInfoUser
        {
            // Public properties
            public int UserID { get; set; }
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(int userID, string username, string firstName, string lastName, string city)
            {
                UserID = userID;
                Username = username;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }
        }

        // Private members
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            // Step 1: Validate the username and the password
            var user = ValidateUserCredentials(authenticationRequestBody.Username, authenticationRequestBody.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            // Step 2: Create a token
            
            // "Header": The security key is used to generate the signing credentials that will
            // be used later to generate the token. The "Header" of the generated token includes
            // those information about what algorithm was used (here the HS256) and the type of it.
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            // "Payload": The claims are part of the "Payload" of the token that will be generated
            // and can include any information we want (sub, given_name, family_name are
            // conventions that are widely used in the claims, that's why we use them here).
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserID.ToString()));
            claimsForToken.Add(new Claim("given_name", user.FirstName));
            claimsForToken.Add(new Claim("family_name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));

            // Here the toke is generated. The last part of the token, the "Signature"
            // is added in the end of the token.
            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);
            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            // IMPORTANT INFO: The tokens are not encrypted but they are only encoded.
            // That means token-based security relies on HTTPS protocol for encryption.
            // This is why, when tokens are used, HTTPS is a must-use.
            return Ok(tokenToReturn);
        }

        private CityInfoUser ValidateUserCredentials(string? username, string? password)
        {
            // Note: In this project we don't have a table in the "CityInfo" DB or
            // a separate DB that store the credentials of the users, so that we can
            // validate the passed-through username and password values from the HTTP
            // POST request against to what is stored inside that DB. Therefore here
            // we assume that the passed credentials from any requests are valid.

            return new CityInfoUser(1, "Agleoras", "Agis", "Surname", "NYC");
        }
    }
}
