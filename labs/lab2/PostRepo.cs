using System;
using Npgsql;
using System.Collections.Generic;

namespace lab2
{
    public class PostRepo
    {
        NpgsqlConnection connection;
        public PostRepo(NpgsqlConnection connection)
        {
            this.connection = connection;
        }
        public static Post GetPost(NpgsqlDataReader reader)
        {
            // NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;Database=lab1db;User Id=postgres;Password=wake up call;");
            // conn.Open();

            Post post = new Post();
            post.postId = reader.GetInt32(0);
            post.postText = reader.GetString(1);
            post.userId = reader.GetInt32(2);
            try
            {
                post.commentId = reader.GetInt32(3);
            }
            catch
            {
                post.commentId = 0;
            }


            return post;
        }
        public Post GetById(int id)
        {
            string command = $"SELECT * FROM posts WHERE post_id = {id}";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();
            Post post = new Post();

            if (reader.Read())
            {
                post.postId = reader.GetInt32(0);
                post.postText = reader.GetString(1);
                post.userId = reader.GetInt32(2);
                post.commentId = reader.GetInt32(3);
            }
            reader.Close();
            return post;
        }
        public bool Insert(Post post)
        {
            Console.WriteLine("insert post");
            string command = $"INSERT INTO posts (post_id, post_text, user_id, comment_id) VALUES ({post.postId}, '{post.postText}', {post.userId}, {post.commentId});";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            cmd.ExecuteScalar();

            UserRepo userRepo = new UserRepo(connection);
            User user = userRepo.GetById(post.userId);
            if (user.username == null)
            {
                user.userId = post.userId;
                user.commentId = 0;

                connection.Close();
                connection.Open();
                command = $"SELECT md5(random()::text);";
                cmd = new NpgsqlCommand(command, connection);
                user.username = cmd.ExecuteScalar().ToString();
                connection.Close();
                connection.Open();
            }
            user.postId = post.postId;
            userRepo.Insert(user);
            return true;
        }
        public bool DeleteById(int id)
        {
            string command = $"DELETE FROM posts WHERE post_id = {id};";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            int res = cmd.ExecuteNonQuery();

            UserRepo userRepo = new UserRepo(connection);
            userRepo.DeleteByPostId(id);

            if (res == -1) return false;
            return true;
        }
        public void DeleteByUserId(int userId)
        {
            string command = $"DELETE FROM posts WHERE user_id = {userId};";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            cmd.ExecuteNonQuery();
        }
        public void DeleteByCommentId(int commentId)
        {
            string command = $"DELETE FROM posts WHERE comment_id = {commentId};";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            cmd.ExecuteNonQuery();
        }
        public bool Update(int postId, Post updatedPost)
        {
            string command = $"UPDATE posts SET post_text = '{updatedPost.postText}' WHERE post_id = {postId};";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            cmd.ExecuteReader();
            return true;
        }
        public void GenerateXRandomPosts(int number)
        {
            UserRepo userRepo = new UserRepo(connection);
            int id = GetNewId();

            for (int i = 0; i < number; i++)
            {
                Post post = new Post();
                post.postId = id + i;

                string command = $"SELECT md5(random()::text);";
                NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
                post.postText = cmd.ExecuteScalar().ToString();
                connection.Close();
                connection.Open();

                post.userId = userRepo.GetNewId();

                this.Insert(post);
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
            id = id + 1;
            return id;
        }
        public List<string> SearchByText(string text)
        {
            List<string> posts = new List<string>();

            string command = $"SELECT * FROM posts WHERE post_text LIKE '%' || '{text}' || '%';";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string currText = "";
                currText = reader.GetString(1);
                if (!posts.Contains(currText))
                {
                    posts.Add(currText);
                }
            }
            return posts;
        }
        public List<Post> GetAll()
        {
            string command = "SELECT * FROM posts";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            List<Post> posts = new List<Post>();
            while (reader.Read())
            {
                Post post = GetPost(reader);
                posts.Add(post);
            }
            connection.Close();
            connection.Open();
            return posts;
        }
    }
}