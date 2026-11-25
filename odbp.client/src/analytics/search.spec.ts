import { describe, expect, test } from "vitest";
import { maskSearchTerm } from "./search";

describe("maskSearchTerm", () => {
  test.each([
    ["klimaat beleid", "klimaat beleid"],
    ["bsn 123456782 woo", "bsn [nummer] woo"],
    ["123.456.782", "[nummer]"],
    ["123 456 782", "[nummer]"],
    ["jan@example.com besluit", "[e-mail] besluit"],
    ["bel 06-12345678", "bel [telefoon]"],
    ["+31 6 12345678", "[telefoon]"],
    ["bel (020) 1234567", "bel [telefoon]"],
    ["(020) 123 45 67", "[telefoon]"],
    ["(0123) 123456", "[telefoon]"],
    ["bel +31 (0)20 1234567", "bel [telefoon]"],
    ["0031 (0)6 12345678", "[telefoon]"],
    ["1234 AB 12", "[adres]"],
    ["1234AB 12a", "[adres]"],
    ["1234AB12", "[adres]"],
    ["1234 AB12", "[adres]"],
    ["besluit 2024", "besluit 2024"],
    ["1234567890123", "1234567890123"],
    ["  veel   spaties ", "veel spaties"]
  ])("%s", (term, expected) => expect(maskSearchTerm(term)).toBe(expected));

  test("limits the length", () => expect(maskSearchTerm("a".repeat(300))).toHaveLength(200));
});
