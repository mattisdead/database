using System;
using Npgsql;
using System.Collections.Generic;
using static System.Console;

namespace lab2
{
    class Program
    {
        static CommentRepo commentRepo;
        static PostRepo postRepo;
        static UserRepo userRepo;

        static void Main(string[] args)
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;Database=lab1db;User Id=postgres;Password=wake up call;");
            conn.Open();
            commentRepo = new CommentRepo(conn);
            postRepo = new PostRepo(conn);
            userRepo = new UserRepo(conn);

            WriteLine("Enter your username:");
            string username = ReadLine();
            User mainUser = userRepo.GetByUsername(username);
            if (mainUser.username == null)
            {
                mainUser.userId = userRepo.GetNewId();
                mainUser.username = username;
                userRepo.Insert(mainUser);
            }

            ProcessCommands process = new ProcessCommands(conn, mainUser);
            WriteLine("Enter \"help\" to see list of commands");
            while (true)
            {
                WriteLine("Enter your command:");
                string command = ReadLine();
                bool exited = false;
                bool res = true;

                if (command.Equals("Add post", StringComparison.InvariantCultureIgnoreCase))
                {
                    res = process.AddPost();
                }
                else if (command.Equals("Add comment", StringComparison.InvariantCultureIgnoreCase))
                {
                    res = process.AddComment(out exited);
                    if (exited) continue;
                }
                else if (command.Equals("Edit post", StringComparison.InvariantCultureIgnoreCase))
                {
                    res = process.EditPost(out exited);
                    if (exited) continue;
                }
                else if (command.Equals("Edit comment", StringComparison.InvariantCultureIgnoreCase))
                {
                    res = process.EditComment(out exited);
                    if (exited) continue;
                }
                else if (command.Equals("Delete post", StringComparison.InvariantCultureIgnoreCase))
                {
                    res = process.DeletePost(out exited);
                    if (exited) continue;
                }
                else if (command.Equals("Delete comment", StringComparison.InvariantCultureIgnoreCase))
                {
                    res = process.DeleteComment(out exited);
                    if (exited) continue;
                }
                else if (command.Equals("Random posts", StringComparison.InvariantCultureIgnoreCase))
                {
                    res = process.GenerateRandomPosts(out exited);
                    if (exited) continue;
                }
                else if (command.Equals("Random comments", StringComparison.InvariantCultureIgnoreCase))
                {
                    res = process.GenerateRandomComments(out exited);
                    if (exited) continue;
                }
                else if (command.Equals("Search posts", StringComparison.InvariantCultureIgnoreCase))
                {
                    process.SearchPosts();
                }
                else if (command.Equals("Search comments", StringComparison.InvariantCultureIgnoreCase))
                {
                    process.SearchComments();
                }
                else if (command.Equals("Show all posts", StringComparison.InvariantCultureIgnoreCase))
                {
                    List<Post> posts = postRepo.GetAll();
                    for (int i = 0; i < posts.Count; i++) WriteLine(posts[i].postText);
                }
                else if (command.Equals("Show all comments", StringComparison.InvariantCultureIgnoreCase))
                {
                    List<Comment> comments = commentRepo.GetAll();
                    for (int i = 0; i < comments.Count; i++) WriteLine(comments[i].commentText);
                }
                else if (command.Equals("Help", StringComparison.InvariantCultureIgnoreCase))
                {
                    PrintHelp();
                }
                else if (command.Equals("Exit", StringComparison.InvariantCultureIgnoreCase)) break;
                else WriteLine("Invalid command");
                if (!res) WriteLine("Something went wrong...");
                conn.Close();
                conn.Open();
            }
            conn.Close();
        }
        static void PrintHelp()
        {
            WriteLine(@"Add post
Add comment
Edit post
Edit comment
Delete post
Delete comment
Random Posts
Random Comments
Search posts
Search comments
Show all posts
Show all comments
Help
EXit");
        }
    }
}
