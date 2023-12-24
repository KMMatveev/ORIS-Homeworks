namespace MyHTTPServer.attributes;

public class PostAttribute : Attribute, IHttpMethodAttribute
{
    public PostAttribute(string actionName)
    {
        ModelName = actionName;
    }

    public string ModelName { get; }
}