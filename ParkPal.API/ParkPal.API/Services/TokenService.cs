using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using ParkPal.API.Services.Interfaces;
using ParkPal.Common.Database.Contexts;
using ParkPal.Common.Models.Database.Entities.Device;

namespace ParkPal.API.Services
{

    public class TokenService : ITokenService
    {
        private DatabaseContext _context;

        public TokenService(DatabaseContext context)
        {
            _context = context;
        }

        public bool Verify(string token)
        {
            return _context.Tokens.Any(a => a.Value == token);
        }

        public Token? GetByToken(string token)
        {
            return _context.Tokens.FirstOrDefault(a => a.Value == token);
        }
        
        public Token? Generate()
        {
            string tokenString = Guid.NewGuid().ToString();
            Token existingToken = GetByToken(tokenString);

            while (existingToken != null)
            {
                tokenString = Guid.NewGuid().ToString();
                existingToken = GetByToken(tokenString);
            }
            
            Token createdToken = new Token()
            {
                Value = tokenString
            };

            _context.Tokens.Add(createdToken);
            _context.SaveChanges();

            return createdToken;
        }
    }
}