using Npgsql;
using System;
using System.Collections.Generic;

namespace lab2
{
    public class UserRepo
    {
        NpgsqlConnection connection;
        public UserRepo(NpgsqlConnection connection)
        {
            this.connection = connection;
        }
        public static User GetUser(NpgsqlDataReader reader)
        {
            // NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;Database=lab1db;User Id=postgres;Password=wake up call;");
            // conn.Open();

            User user = new User();
            user.userId = reader.GetInt32(0);
            user.username = reader.GetString(1);
            user.postId = reader.GetInt32(2);
            user.commentId = reader.GetInt32(3);

            return user;
        }
        public User GetById(int id)
        {
            string command = $"SELECT * FROM users WHERE user_id = {id}";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();
            User user = new User();

            if (reader.Read())
            {
                user.userId = reader.GetInt32(0);
                user.username = reader.GetString(1);
                user.postId = reader.GetInt32(2);
                user.commentId = reader.GetInt32(3);
            }
            reader.Close();
            return user;
        }
        public User GetByUsername(string username)
        {
            string command = $"SELECT * FROM users WHERE username = '{username}';";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();
            User user = new User();

            if (reader.Read())
            {
                user.userId = reader.GetInt32(0);
                user.username = reader.GetString(1);
                user.postId = reader.GetInt32(2);
                user.commentId = reader.GetInt32(3);
            }
            reader.Close();
            return user;
        }
        public bool Insert(User user)
        {
            string command = $"INSERT INTO users (user_id, username, post_id, comment_id) VALUES ({user.userId}, '{user.username}', {user.postId}, {user.commentId});";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            if (cmd.ExecuteScalar() == null) return false;
            return true;
        }
        public bool DeleteById(int id)
        {
            string command = $"DELETE FROM users WHERE user_id = {id};";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            int res = cmd.ExecuteNonQuery();

            PostRepo postRepo = new PostRepo(connection);
            postRepo.DeleteByUserId(id);

            CommentRepo commentRepo = new CommentRepo(connection);
            commentRepo.DeleteByUserId(id);

            if (res == -1) return false;
            return true;
        }
        public void DeleteByCommentId(int commentId)
        {
            string command = $"DELETE FROM users WHERE comment_id = {commentId};";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            cmd.ExecuteNonQuery();
        }
        public void DeleteByPostId(int postId)
        {
            string command = $"DELETE FROM users WHERE post_id = {postId};";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            cmd.ExecuteNonQuery();
        }
        public void GenerateXRandomUsers(int number)
        {
            int id = GetNewId();

            for (int i = 0; i < number; i++)
            {
                User user = new User();
                user.userId = id + i;

                string command = $"SELECT md5(random()::text);";
                NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
                user.username = cmd.ExecuteScalar().ToString();
                connection.Close();
                connection.Open();

                this.Insert(user);
            }
        }
        public int GetNewId()
        {
            string command = $"SELECT * FROM comments;";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();

            int id = 1;
            while (reader.Read())
            {
                int currId = reader.GetInt32(0);
                if (currId > id) id = currId;
            }
            connection.Close();
            connection.Open();
            return id += 1;
        }
        public List<string> SearchByUsername(string username)
        {
            List<string> users = new List<string>();

            string command = $"SELECT * FROM users WHERE username LIKE '%' || '{username}' || '%';";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string currUsername = "";
                currUsername = reader.GetString(1);
                if (!users.Contains(currUsername))
                {
                    users.Add(currUsername);
                }
            }
            return users;
        }
        public List<User> GetAll()
        {
            string command = "SELECT * FROM users";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            List<User> users = new List<User>();
            while (reader.Read())
            {
                User user = GetUser(reader);
                users.Add(user);
            }
            connection.Close();
            connection.Open();
            return users;
        }
    }
}