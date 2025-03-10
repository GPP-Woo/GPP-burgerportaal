import DOMPurify from "dompurify";
import sprite from "@/assets/icon-sprite.svg";

class AnchorModifier {
  node: HTMLAnchorElement;

  constructor(node: HTMLAnchorElement) {
    this.node = node;
  }

  addClass = (...c: string[]) => (this.node.classList.add(...c), this);

  addRelationships = (...r: string[]) => (this.node.relList.add(...r), this);

  appendHiddenText = (t: string) => (
    this.node.appendChild(
      Object.assign(document.createElement("span"), {
        className: "visually-hidden",
        innerText: t
      })
    ),
    this
  );

  appendIconExternal = () => {
    const span = document.createElement("span");
    span.classList.add("utrecht-icon");
    span.setAttribute("role", "presentation");
    span.setAttribute("aria-hidden", "true");

    const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
    svg.setAttribute("fill", "currentColor");

    const use = document.createElementNS("http://www.w3.org/2000/svg", "use");
    use.setAttributeNS("http://www.w3.org/1999/xlink", "xlink:href", `${sprite}#external`);

    this.node.appendChild(span).appendChild(svg).appendChild(use);

    return this;
  };
}

const modifyExternalLinks = (html: string) => {
  const container = Object.assign(document.createElement("div"), { innerHTML: html });

  ((container.querySelectorAll("a") || []) as NodeListOf<HTMLAnchorElement>).forEach(
    (anchor) =>
      anchor.origin !== location.origin &&
      new AnchorModifier(anchor)
        .addClass("gpp-woo-link--icon")
        .addRelationships("external", "noopener", "noreferrer")
        .appendHiddenText("(externe link)")
        .appendIconExternal()
  );

  return container.innerHTML;
};

DOMPurify.addHook("afterSanitizeAttributes", (node) => {
  if (node.tagName === "H1") node.classList.add("utrecht-heading-1");
  if (node.tagName === "H2") node.classList.add("utrecht-heading-2");
  if (node.tagName === "P") node.classList.add("utrecht-paragraph");
  if (node.tagName === "A") node.classList.add("utrecht-link", "utrecht-link--html-a");
  if (node.tagName === "UL") node.classList.add("utrecht-unordered-list");
  if (node.tagName === "OL") node.classList.add("utrecht-ordered-list");
  if (node.tagName === "LI")
    node.classList.add(
      node.parentNode?.nodeName === "OL"
        ? "utrecht-ordered-list__item"
        : "utrecht-unordered-list__item"
    );
});

export function sanitizeHtml(dirtyHtml: string) {
  const cleanHtml = DOMPurify.sanitize(dirtyHtml, {
    ALLOWED_TAGS: ["h1", "h2", "p", "a", "ul", "ol", "li"],
    ALLOWED_ATTR: ["href"]
  });

  return modifyExternalLinks(cleanHtml);
}

export function sanitizeSvg(dirtySvg: string) {
  return DOMPurify.sanitize(dirtySvg, {USE_PROFILES: {svg: true, svgFilters: true}});
}
