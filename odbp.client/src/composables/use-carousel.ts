import { ref, computed, watch, onMounted, type MaybeRefOrGetter, toRef } from "vue";
import { useScroll, useElementSize, useEventListener, useTimeoutFn } from "@vueuse/core";

export type UseCarouselOptions = {
  autoplayInterval?: number;
  autoplayInitialState?: boolean;
};

export const useCarousel = <T>(items: MaybeRefOrGetter<T[]>, options: UseCarouselOptions = {}) => {
  const { autoplayInterval = 5000, autoplayInitialState = true } = options;

  const itemsRef = toRef(items);

  // Duplicate the items array on both sides to facilitate the infinite scrolling effect
  const infiniteItems = computed(() => [...itemsRef.value, ...itemsRef.value, ...itemsRef.value]);

  // DOM ref to scrollContainer
  const scrollContainer = ref<HTMLElement | null>(null);

  // Scroll position and dimensions
  const { isScrolling, x: scrollLeft } = useScroll(scrollContainer);
  const { width: containerWidth } = useElementSize(scrollContainer);

  // Recalculate the slideWidth (item width + gap) as the containerWidth changes
  const slideWidth = ref(0);

  watch(containerWidth, () => {
    if (!scrollContainer.value) return;

    const itemWidth = scrollContainer.value.querySelector("li")?.offsetWidth || 0;
    const columnGap = parseInt(getComputedStyle(scrollContainer.value).columnGap);

    slideWidth.value = itemWidth + columnGap;
  });

  // Current index tracking
  const currentIndex = computed(() => Math.round(scrollLeft.value / slideWidth.value));
  const normalizedIndex = computed(() => currentIndex.value % itemsRef.value.length);
  const visibleItemsCount = computed(() => Math.round(containerWidth.value / slideWidth.value));

  // Handle infinite scroll behavior
  const handleInfiniteScroll = () => {
    let index = null;

    if (currentIndex.value <= 0) {
      index = currentIndex.value + itemsRef.value.length;
    } else if (currentIndex.value >= infiniteItems.value.length - itemsRef.value.length) {
      index = currentIndex.value - itemsRef.value.length;
    }

    if (index !== null) {
      scrollToIndex(index, false);
    }
  };

  // Handle focusable elements inside slides
  const handleFocusables = () => {
    if (!scrollContainer.value) return;

    const slides = scrollContainer.value.querySelectorAll("li");

    slides.forEach((slide) => {
      const focusableElements = slide.querySelectorAll(
        `a, button, input, select, textarea, [tabindex]:not([tabindex="-1"])`
      );

      focusableElements.forEach((el) => {
        if (slide.getAttribute("aria-hidden") === "true") {
          el.setAttribute("tabindex", "-1");
        } else {
          el.removeAttribute("tabindex");
        }
      });
    });
  };

  // Initialize carousel when dimensions are available
  watch(
    () => containerWidth.value > 0,
    () => {
      handleInfiniteScroll();
      handleFocusables();
    },
    { once: true }
  );

  // Scroll listener
  const shouldResetFocus = ref(false);

  watch(isScrolling, (scrolling) => {
    if (!scrolling) {
      // Scrolling stopped
      handleInfiniteScroll();
      handleFocusables();

      if (shouldResetFocus.value) {
        // Set focus to scrollContainer only after scrolling to keep proper keyboard navigation
        scrollContainer.value?.focus();
        shouldResetFocus.value = false;
      }
    } else {
      // Scrolling started
      if (
        document.activeElement instanceof HTMLElement &&
        scrollContainer.value?.contains(document.activeElement)
      ) {
        // Remove focus from carousel item before aria-hidden is applied
        document.activeElement.blur();
        shouldResetFocus.value = true;
      }
    }
  });

  // Item visibility
  const isItemVisible = (index: number) => {
    const visibleIndices = Array.from(
      { length: visibleItemsCount.value },
      (_, i) => currentIndex.value + i
    );

    return visibleIndices.includes(index);
  };

  // Navigation
  const scrollToIndex = (index: number, smooth = true) => {
    if (!scrollContainer.value) return;

    scrollContainer.value.scrollTo({
      left: index * slideWidth.value,
      behavior: smooth ? "smooth" : "auto"
    });

    if (autoplayEnabled.value) {
      stopAutoplay();
      startAutoplay();
    }
  };

  const scrollPrev = () => scrollToIndex(currentIndex.value - 1);
  const scrollNext = () => scrollToIndex(currentIndex.value + 1);

  // Autoplay
  const autoplayEnabled = ref(autoplayInitialState);

  const { start: startAutoplay, stop: stopAutoplay } = useTimeoutFn(() => {
    if (!autoplayEnabled.value) return;

    scrollNext();
    startAutoplay();
  }, autoplayInterval);

  const toggleAutoplay = () => {
    autoplayEnabled.value = !autoplayEnabled.value;

    if (autoplayEnabled.value) {
      startAutoplay();
    } else {
      stopAutoplay();
    }
  };

  // Initialize autoplay
  onMounted(() => autoplayEnabled.value && startAutoplay());

  // Pause autoplay
  useEventListener(scrollContainer, ["mouseenter", "touchstart", "focusin"], stopAutoplay, {
    passive: true
  });

  // Resume autoplay
  useEventListener(
    scrollContainer,
    ["mouseleave", "touchend", "touchcancel"],
    () => autoplayEnabled.value && startAutoplay(),
    {
      passive: true
    }
  );

  useEventListener(
    scrollContainer,
    "focusout",
    (e: FocusEvent) =>
      !scrollContainer.value?.contains(e.relatedTarget as Node) &&
      autoplayEnabled.value &&
      startAutoplay()
  );

  return {
    // DOM ref
    scrollContainer,

    // Computed/reactive properties
    infiniteItems,
    currentIndex,
    normalizedIndex,
    visibleItemsCount,
    autoplayEnabled,

    // Methods
    isItemVisible,
    scrollToIndex,
    scrollPrev,
    scrollNext,
    toggleAutoplay
  };
};
