namespace MyHTTPServer.attributes;

public class GetAttribute : Attribute, IHttpMethodAttribute
{
    public GetAttribute(string actionName)
    {
        ModelName = actionName;
    }

    public string ModelName { get; }
}