namespace octo
{
    internal class LinkInformationComparer : IEqualityComparer<LinkInformation>
    {
        public bool Equals(LinkInformation x, LinkInformation y)
        {
            return StringComparer
                .InvariantCultureIgnoreCase
                .Equals(x.Uri.OriginalString, y.Uri.OriginalString);
        }

        public int GetHashCode(LinkInformation pageInformation)
        {
            return StringComparer
                .InvariantCultureIgnoreCase
                .GetHashCode(pageInformation.Uri.OriginalString);
        }
    }
}
