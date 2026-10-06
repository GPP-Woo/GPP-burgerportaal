import { describe, expect, it } from "vitest";
import { formatCappedCount } from "./numbers";

describe("formatCappedCount", () => {
  it("formats a count below the threshold with Dutch thousands notation", () => {
    expect(formatCappedCount(1234, 9_999)).toBe("1.234");
  });

  it("formats a count of exactly the threshold as the exact value", () => {
    expect(formatCappedCount(9_999, 9_999)).toBe("9.999");
  });

  it("caps a count above the threshold as '<threshold>+'", () => {
    expect(formatCappedCount(10_000, 9_999)).toBe("9.999+");
  });

  it("formats zero as '0'", () => {
    expect(formatCappedCount(0, 9_999)).toBe("0");
  });

  it("supports a different threshold for another display location", () => {
    expect(formatCappedCount(100_000, 99_999)).toBe("99.999+");
    expect(formatCappedCount(99_999, 99_999)).toBe("99.999");
  });
});
