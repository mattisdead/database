namespace lab2
{
    public class User
    {
        public int userId;
        public string username;
        public int postId;
        public int commentId;
        public User() : base() { }
        public User(int userId, string username, int postId, int commentId)
        {
            this.userId = userId;
            this.username = username;
            this.postId = postId;
            this.commentId = commentId;
        }
    }
}