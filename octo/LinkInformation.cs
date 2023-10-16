namespace octo
{
    struct LinkInformation
    {
        public Uri Uri { get; }

        public int Depth { get; }

        public LinkInformation(string url, int depth)
        {
            Uri = new Uri(url);
            Depth = depth;
        }
    }
}
