public static class Extensions {
    public static bool IsValid (this string str) {
        return !string.IsNullOrEmpty (str);
    }
}