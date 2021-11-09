using System;
using static System.Console;
using Npgsql;
using System.Collections.Generic;
using System.Diagnostics;

namespace lab2
{
    public class ProcessCommands
    {
        CommentRepo commentRepo;
        PostRepo postRepo;
        UserRepo userRepo;
        User mainUser;
        public ProcessCommands(NpgsqlConnection connection, User mainUser)
        {
            commentRepo = new CommentRepo(connection);
            postRepo = new PostRepo(connection);
            userRepo = new UserRepo(connection);
            this.mainUser = mainUser;
        }
        public bool AddPost()
        {
            WriteLine("Type your post:");
            string text = ReadLine();
            Post post = new Post(postRepo.GetNewId(), text, mainUser.userId, 0);
            return postRepo.Insert(post);
        }
        public bool AddComment(out bool exited)
        {
            WriteLine("What is the id of the post you are commenting?");
            string strId = "";
            Post post = new Post();
            int id = 0;
            exited = false;
            while (true)
            {
                strId = ReadLine();
                if (strId.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    exited = true;
                    break;
                }
                else if (!int.TryParse(strId, out id))
                {
                    WriteLine("Enter an integer, please");
                    continue;
                }
                post = postRepo.GetById(id);
                if (post.postText == null)
                {
                    WriteLine("This post does not exists");
                    continue;
                }
                break;
            }
            if (exited) return false;
            WriteLine("\r\nType your comment:");
            string text = ReadLine();
            Comment comment = new Comment(commentRepo.GetNewId(), text, mainUser.userId, post.postId);
            bool res = commentRepo.Insert(comment);
            return res;
        }
        public bool EditPost(out bool exited)
        {
            WriteLine("What post would you like to edit?");
            int id = 0;
            Post post = new Post();

            ProcessPostIdInput(out exited, out id);
            if (exited) return false;
            post = postRepo.GetById(id);

            WriteLine("Write new text for a post:");
            string text = ReadLine();
            post.postText = text;
            return postRepo.Update(id, post);
        }
        public bool EditComment(out bool exited)
        {
            WriteLine("What comment would you like to edit?");

            int id = 0;
            Comment comment = new Comment();

            ProcessCommentIdInput(out exited, out id);
            comment = commentRepo.GetById(id);
            if (exited) return false;

            WriteLine("Write new text for a comment:");
            string text = ReadLine();
            comment.commentText = text;
            return commentRepo.Update(id, comment);
        }
        public bool DeletePost(out bool exited)
        {
            WriteLine("What post would you like to delete?");
            int id = 0;
            Post post = new Post();

            ProcessPostIdInput(out exited, out id);
            if (exited) return false;
            return postRepo.DeleteById(id);
        }
        public bool DeleteComment(out bool exited)
        {
            WriteLine("What comment would you like to delete?");
            int id = 0;
            Comment comment = new Comment();

            ProcessCommentIdInput(out exited, out id);
            if (exited) return false;
            return commentRepo.DeleteById(id);
        }
        public bool GenerateRandomPosts(out bool exited)
        {
            exited = false;
            WriteLine("How many posts would you like to generate?");
            string strNumber;
            int number = 0;
            while (true)
            {
                strNumber = ReadLine();
                if (strNumber.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    exited = true;
                    break;
                }
                if (!int.TryParse(strNumber, out number))
                {
                    WriteLine("Enter an integer, please");
                    continue;
                }
                break;
            }
            if (exited) return false;
            try { postRepo.GenerateXRandomPosts(number); return true; }
            catch { return false; }
        }
        public bool GenerateRandomComments(out bool exited)
        {
            exited = false;
            WriteLine("How many comments would you like to generate?");
            string strNumber;
            int number = 0;
            while (true)
            {
                strNumber = ReadLine();
                if (strNumber.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    exited = true;
                    break;
                }
                if (!int.TryParse(strNumber, out number))
                {
                    WriteLine("Enter an integer, please");
                    continue;
                }
                break;
            }
            if (exited) return false;
            try { commentRepo.GenerateXRandomComments(number); return true; }
            catch { return false; }
        }
        public void SearchPosts()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            WriteLine("Enter search input:");
            string search = ReadLine();
            List<string> posts = postRepo.SearchByText(search);
            if (posts.Count == 0)
            {
                WriteLine("No match");
                return;
            }
            for (int i = 0; i < posts.Count; i++)
            {
                WriteLine(posts[i]);
            }
            WriteLine();
            stopwatch.Stop();
            TimeSpan stopwatchElapsed = stopwatch.Elapsed;
            Console.WriteLine("Total time in ms: " + Convert.ToInt32(stopwatchElapsed.TotalMilliseconds));
        }
        public void SearchComments()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            WriteLine("Enter search input:");
            string search = ReadLine();
            List<string> comments = commentRepo.SearchByText(search);
            if (comments.Count == 0)
            {
                WriteLine("No match");
                return;
            }
            for (int i = 0; i < comments.Count; i++)
            {
                WriteLine(comments[i]);
            }
            WriteLine();
            stopwatch.Stop();
            TimeSpan stopwatchElapsed = stopwatch.Elapsed;
            Console.WriteLine("Total time in ms: " + Convert.ToInt32(stopwatchElapsed.TotalMilliseconds));
        }
        void ProcessPostIdInput(out bool exited, out int id)
        {
            exited = false;
            id = 0;
            while (true)
            {
                string strId = ReadLine();
                if (strId.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    exited = true;
                    break;
                }
                else if (!int.TryParse(strId, out id))
                {
                    WriteLine("Enter an integer, please");
                    continue;
                }
                Post post = postRepo.GetById(id);
                if (post.postText == null)
                {
                    WriteLine("This post does not exists");
                    continue;
                }
                break;
            }
        }
        void ProcessCommentIdInput(out bool exited, out int id)
        {
            exited = false;
            id = 0;
            while (true)
            {
                string strId = ReadLine();
                if (strId.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    exited = true;
                    break;
                }
                else if (!int.TryParse(strId, out id))
                {
                    WriteLine("Enter an integer, please");
                    continue;
                }
                Comment comment = commentRepo.GetById(id);
                if (comment.commentText == null)
                {
                    WriteLine("This post does not exists");
                    continue;
                }
                break;
            }
        }
    }
}