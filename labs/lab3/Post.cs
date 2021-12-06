namespace lab3
{
    public class Post
    {
        public int postId;
        public string postText;
        public int userId;
        public int commentId;
        public Post() : base() { }
        public Post(int postId, string postText, int userId, int commentId)
        {
            this.postId = postId;
            this.postText = postText;
            this.commentId = commentId;
            this.userId = userId;
        }
    }
}