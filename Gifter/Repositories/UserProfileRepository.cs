﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Gifter.Models;
using Gifter.Utils;

namespace Gifter.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration configuration) : base(configuration) { }

        public List<UserProfile> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT Id, [Name], Email, ImageUrl, DateCreated
                  FROM UserProfile 
              ORDER BY DateCreated";

                    var reader = cmd.ExecuteReader();

                    var users = new List<UserProfile>();
                    while (reader.Read())
                    {
                        users.Add(new UserProfile()
                        {
                            Id = DbUtils.GetInt(reader, "Id"),
                            Name = DbUtils.GetString(reader, "Name"),
                            Email = DbUtils.GetString(reader, "Email"),
                            ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                            DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                        });
                    }

                    reader.Close();

                    return users;
                }
            }
        }

        public UserProfile GetById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT Id, [Name], Email, ImageUrl, DateCreated
                  FROM UserProfile
              WHERE Id = @id
              ORDER BY DateCreated";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    var reader = cmd.ExecuteReader();

                    UserProfile user = null;
                    if (reader.Read())
                    {
                        user = new UserProfile()
                        {
                            Id = id,
                            Name = DbUtils.GetString(reader, "Name"),
                            Email = DbUtils.GetString(reader, "Email"),
                            DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                            ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                        };
                    }

                    reader.Close();

                    return user;
                }
            }
        }


        public void Add(UserProfile user)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO UserProfile ([Name], Email, DateCreated, ImageUrl)
                        OUTPUT INSERTED.ID
                        VALUES (@Name, @Email, @DateCreated, @ImageUrl)";

                    DbUtils.AddParameter(cmd, "@Name", user.Name);
                    DbUtils.AddParameter(cmd, "@Email", user.Email);
                    DbUtils.AddParameter(cmd, "@DateCreated", user.DateCreated);
                    DbUtils.AddParameter(cmd, "@ImageUrl", user.ImageUrl);

                    user.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(UserProfile user)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE UserProfile
                           SET [Name] = @Name,
                               Email = @Email,
                               DateCreated = @DateCreated,
                               ImageUrl = @ImageUrl,
                         WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Name", user.Name);
                    DbUtils.AddParameter(cmd, "@Email", user.Email);
                    DbUtils.AddParameter(cmd, "@DateCreated", user.DateCreated);
                    DbUtils.AddParameter(cmd, "@ImageUrl", user.ImageUrl);
                    DbUtils.AddParameter(cmd, "@Id", user.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM UserProfile WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}