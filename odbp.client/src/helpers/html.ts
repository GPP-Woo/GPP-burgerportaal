import DOMPurify from "dompurify";

DOMPurify.addHook("afterSanitizeAttributes", (node) => {
  node.tagName === "H1" && node.classList.add("utrecht-heading-1");
  node.tagName === "H2" && node.classList.add("utrecht-heading-2");
  node.tagName === "P" && node.classList.add("utrecht-paragraph");
  node.tagName === "A" && node.classList.add("utrecht-link", "utrecht-link--html-a");
  node.tagName === "UL" && node.classList.add("utrecht-unordered-list");
  node.tagName === "OL" && node.classList.add("utrecht-ordered-list");
  node.tagName === "LI" &&
    node.classList.add(
      node.parentNode?.nodeName === "OL"
        ? "utrecht-ordered-list__item"
        : "utrecht-unordered-list__item"
    );

  if (node instanceof HTMLAnchorElement) {
    node.classList.add("gpp-woo-link--icon", "gpp-woo-link--external");
    node.relList.add("external", "noopener", "noreferrer");
  }
});

export function sanitizeHtml(html: string) {
  return DOMPurify.sanitize(html, {
    ALLOWED_TAGS: ["h1", "h2", "p", "a", "ul", "ol", "li"],
    ALLOWED_ATTR: ["href"]
  });
}
