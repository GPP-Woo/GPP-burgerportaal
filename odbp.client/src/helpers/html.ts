import DOMPurify from "dompurify";

DOMPurify.addHook("afterSanitizeAttributes", (node) => {
  if (node instanceof HTMLAnchorElement && !node.href.startsWith("#")) {
    node.target = "_blank";
    node.relList.add("noopener", "noreferrer");
  }
});

export function sanitizeHtml(html: string) {
  return DOMPurify.sanitize(html, {
    FORBID_ATTR: ["class", "style"],
    FORBID_TAGS: ["img"]
  });
}
