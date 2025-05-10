using System.Xml.Serialization;

namespace ODBP.Features.Sitemap.SitemapInstances
{
    public class DiwooXmlResult(SitemapModel model) : XmlResult<SitemapModel>(model, s_namespaces, s_schemas)
    {
        private static readonly XmlSerializerNamespaces s_namespaces = GetNamespaces();
        private static readonly (string, string)[] s_schemas =
        [
            (DiwooConstants.DiwooNamespace, "https://standaarden.overheid.nl/diwoo/metadata/0.9.4/xsd/diwoo-metadata-lijsten.xsd"),
            (DiwooConstants.DiwooNamespace, "https://standaarden.overheid.nl/diwoo/metadata/0.9.4/xsd/diwoo-metadata.xsd"),
            (DiwooConstants.SitemapNamespace, "http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd")
        ];

        private static XmlSerializerNamespaces GetNamespaces()
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("diwoo", DiwooConstants.DiwooNamespace);
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            return namespaces;
        }
    }
}
