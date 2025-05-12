import { ref, computed, onMounted, type MaybeRefOrGetter, toRef } from "vue";
import {
  useScroll,
  useElementSize,
  useResizeObserver,
  useEventListener,
  useTimeoutFn
} from "@vueuse/core";

export type UseCarouselOptions = {
  autoplayInterval?: number;
  autoplayInitialState?: boolean;
};

export const useCarousel = <T>(items: MaybeRefOrGetter<T[]>, options: UseCarouselOptions = {}) => {
  const { autoplayInterval = 5000, autoplayInitialState = true } = options;

  const itemsRef = toRef(items);

  // Create infinite items array
  const infiniteItems = computed(() =>
    itemsRef.value.length > visibleItemsCount.value
      ? [...itemsRef.value, ...itemsRef.value, ...itemsRef.value]
      : itemsRef.value
  );

  // DOM refs
  const scrollContainer = ref<HTMLElement | null>(null);
  const firstItem = ref<HTMLElement | null>(null);

  // Scroll position and dimensions
  const { x: scrollLeft } = useScroll(scrollContainer);
  const { width: containerWidth } = useElementSize(scrollContainer);
  const { width: itemWidth } = useElementSize(firstItem);

  // Calculate width of each slide (item width + gap)
  const slideWidth = computed(() => {
    if (!scrollContainer.value) return 1;

    return itemWidth.value + parseInt(getComputedStyle(scrollContainer.value).columnGap);
  });

  // Current index tracking
  const currentIndex = computed(() => Math.round(scrollLeft.value / slideWidth.value));
  const normalizedIndex = computed(() => currentIndex.value % itemsRef.value.length);
  const visibleItemsCount = computed(() => Math.round(containerWidth.value / slideWidth.value));

  // Handle infinite scroll behavior
  const handleInfiniteScroll = () => {
    if (!scrollContainer.value) return;

    let index = null;

    if (currentIndex.value <= 0) {
      index = currentIndex.value + itemsRef.value.length;
    } else if (currentIndex.value >= infiniteItems.value.length - itemsRef.value.length) {
      index = currentIndex.value - itemsRef.value.length;
    }

    if (index !== null) {
      scrollContainer.value.style.scrollBehavior = "auto";
      scrollContainer.value.scrollLeft = index * slideWidth.value;
    }
  };

  // Resize and scroll listeners
  useResizeObserver(scrollContainer, () => handleInfiniteScroll());
  useEventListener(scrollContainer, "scrollend", handleInfiniteScroll, { passive: true });

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
    scrollContainer.value?.scrollTo({
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

  // Hover/touch pause autoplay behavior
  useEventListener(scrollContainer, ["mouseenter", "touchstart"], stopAutoplay, { passive: true });
  useEventListener(
    scrollContainer,
    ["mouseleave", "touchend", "touchcancel"],
    () => autoplayEnabled.value && startAutoplay(),
    {
      passive: true
    }
  );

  return {
    // DOM refs for template binding
    scrollContainer,
    firstItem,

    // Computed and reactive properties
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
