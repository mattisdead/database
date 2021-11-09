using System;
using Npgsql;
using System.Collections.Generic;

namespace lab2
{
    public class CommentRepo
    {
        NpgsqlConnection connection;
        public CommentRepo(NpgsqlConnection connection)
        {
            this.connection = connection;
        }
        public static Comment GetComment(NpgsqlDataReader reader)
        {
            // NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;Database=lab1db;User Id=postgres;Password=wake up call;");
            // conn.Open();

            Comment comment = new Comment();
            comment.commentId = reader.GetInt32(0);
            comment.commentText = reader.GetString(1);
            comment.userId = reader.GetInt32(2);
            comment.postId = reader.GetInt32(3);

            return comment;
        }
        public Comment GetById(int id)
        {
            string command = $"SELECT * FROM comments WHERE comment_id = {id}";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();
            Comment comment = new Comment();

            if (reader.Read())
            {
                comment.commentId = reader.GetInt32(0);
                comment.commentText = reader.GetString(1);
                comment.userId = reader.GetInt32(2);
                comment.postId = reader.GetInt32(3);
            }
            reader.Close();
            return comment;
        }
        public bool Insert(Comment comment)
        {
            Console.WriteLine("insert comm");
            string command = $"INSERT INTO comments (comment_id, comment_text, user_id, post_id) VALUES ({comment.commentId}, '{comment.commentText}', {comment.userId}, {comment.postId});";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            //if (cmd.ExecuteScalar() == null) return false;
            cmd.ExecuteScalar();

            UserRepo userRepo = new UserRepo(connection);
            User user = userRepo.GetById(comment.userId);

            if (user.username == null)
            {
                user.userId = comment.userId;
                user.postId = 0;

                connection.Close();
                connection.Open();
                command = $"SELECT md5(random()::text);";
                cmd = new NpgsqlCommand(command, connection);
                user.username = cmd.ExecuteScalar().ToString();
                connection.Close();
                connection.Open();
            }
            user.commentId = comment.commentId;
            userRepo.Insert(user);

            PostRepo postRepo = new PostRepo(connection);
            Post post = postRepo.GetById(comment.postId);
            if (post.postText == null)
            {
                post.postId = comment.postId;

                connection.Close();
                connection.Open();
                command = $"SELECT md5(random()::text);";
                cmd = new NpgsqlCommand(command, connection);
                post.postText = cmd.ExecuteScalar().ToString();
                connection.Close();
                connection.Open();

                post.userId = userRepo.GetNewId();
            }
            post.commentId = comment.commentId;
            postRepo.Insert(post);
            return true;
        }
        public bool DeleteById(int id)
        {
            string command = $"DELETE FROM comments WHERE comment_id = {id};";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            int res = cmd.ExecuteNonQuery();

            PostRepo postRepo = new PostRepo(connection);
            postRepo.DeleteByCommentId(id);

            UserRepo userRepo = new UserRepo(connection);
            userRepo.DeleteByCommentId(id);
            if (res == -1) return false;
            return true;
        }
        public void DeleteByUserId(int userId)
        {
            string command = $"DELETE FROM comments WHERE user_id = {userId};";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            cmd.ExecuteNonQuery();
        }
        public bool Update(int commentId, Comment updatedComment)
        {
            string command = $"UPDATE comments SET comment_text = '{updatedComment.commentText}' WHERE comment_id = {commentId};";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            cmd.ExecuteReader();
            return true;
        }
        public void GenerateXRandomComments(int number)
        {
            UserRepo userRepo = new UserRepo(connection);
            int id = GetNewId();

            for (int i = 0; i < number; i++)
            {
                Comment comment = new Comment();
                PostRepo postRepo = new PostRepo(connection);
                comment.commentId = id + i;

                string command = $"SELECT md5(random()::text);";
                NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
                comment.commentText = cmd.ExecuteScalar().ToString();
                connection.Close();
                connection.Open();

                comment.userId = userRepo.GetNewId();
                comment.postId = postRepo.GetNewId();

                this.Insert(comment);
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
            List<string> comments = new List<string>();

            string command = $"SELECT * FROM comments WHERE comment_text LIKE '%' || '{text}' || '%';";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string currText = "";
                currText = reader.GetString(1);
                if (!comments.Contains(currText))
                {
                    comments.Add(currText);
                }
            }
            return comments;
        }
        public List<string> SearchByPostId(int id)
        {
            List<string> replies = new List<string>();

            string command = $"SELECT * FROM comments WHERE post_id BETWEEN {id} AND {id};";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string currComment = "";
                currComment = reader.GetString(1);
                if (!replies.Contains(currComment))
                {
                    replies.Add(currComment);
                }
            }
            connection.Close();
            connection.Open();
            return replies;
        }
        public List<Comment> GetAll()
        {
            string command = "SELECT * FROM comments";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            List<Comment> comments = new List<Comment>();
            while (reader.Read())
            {
                Comment comment = GetComment(reader);
                comments.Add(comment);
            }
            connection.Close();
            connection.Open();
            return comments;
        }
    }
}