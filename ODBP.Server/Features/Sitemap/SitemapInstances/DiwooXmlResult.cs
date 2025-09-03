using System.Xml.Serialization;

namespace ODBP.Features.Sitemap.SitemapInstances
{
    public class DiwooXmlResult(SitemapModel model) : XmlResult<SitemapModel>(model, s_namespaces, s_schemas)
    {
        private static readonly XmlSerializerNamespaces s_namespaces = GetNamespaces();
        private static readonly (string, string)[] s_schemas =
        [
            (DiwooConstants.DiwooNamespace, "https://standaarden.overheid.nl/diwoo/metadata/0.9.8/xsd/diwoo/diwoo-metadata-lijsten.xsd"),
            (DiwooConstants.DiwooNamespace, "https://standaarden.overheid.nl/diwoo/metadata/0.9.8/xsd/diwoo/diwoo-metadata.xsd"),
            (DiwooConstants.SitemapNamespace, "https://standaarden.overheid.nl/diwoo/metadata/0.9.8/xsd/extern/sitemap/sitemap.xsd"),
            (DiwooConstants.MdtoNamespace, "https://standaarden.overheid.nl/diwoo/metadata/0.9.8/xsd/extern/mdto/MDTO-XML1.0.1.xsd")
        ];

        private static XmlSerializerNamespaces GetNamespaces()
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("diwoo", DiwooConstants.DiwooNamespace);
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            namespaces.Add("mdto", DiwooConstants.MdtoNamespace);
            return namespaces;
        }
    }
}
