import { createRouter, createWebHistory, type NavigationGuard } from "vue-router";
import { fetchAuthUser } from "@/api/auth";
import HomeView from "../views/HomeView.vue";
import SearchView from "../views/SearchView.vue";

const requiresAdmin: NavigationGuard = async (to) => {
  const user = await fetchAuthUser();

  if (!user?.isLoggedIn) {
    window.location.href = `/api/challenge?returnUrl=${encodeURIComponent(to.fullPath)}`;

    return false;
  }

  if (!user?.isAdmin && to.name !== "forbidden") return { name: "forbidden" };
};

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "home",
      component: HomeView,
      meta: {
        title: "Homepage"
      }
    },
    {
      path: "/zoeken",
      name: "zoeken",
      component: SearchView,
      meta: {
        title: "Zoeken"
      }
    },
    {
      path: "/publicaties/:uuid",
      name: "publicatie",
      component: () => import("../views/PublicatieView.vue"),
      props: true,
      meta: {
        title: "Publicatie"
      }
    },
    {
      path: "/documenten/:uuid",
      name: "document",
      component: () => import("../views/DocumentView.vue"),
      props: true,
      meta: {
        title: "Document"
      }
    },
    {
      path: "/onderwerpen",
      name: "onderwerpen",
      component: () => import("../views/OnderwerpenView.vue"),
      props: true,
      meta: {
        title: "Onderwerpen"
      }
    },
    {
      path: "/onderwerpen/:uuid",
      name: "onderwerp",
      component: () => import("../views/OnderwerpView.vue"),
      props: true,
      meta: {
        title: "Onderwerp"
      }
    },
    {
      path: "/beheer",
      name: "beheer",
      component: () => import("../views/beheer/BeheerLayout.vue"),
      beforeEnter: requiresAdmin,
      children: [
        {
          path: "",
          name: "beheer-home",
          component: () => import("../views/beheer/BeheerHomeView.vue"),
          meta: {
            title: "Beheer - Homepage"
          }
        },
        {
          path: "afbeeldingen",
          name: "beheer-afbeeldingen",
          component: () => import("../views/beheer/BeheerAfbeeldingenView.vue"),
          meta: {
            title: "Beheer - Afbeeldingen"
          }
        },
        {
          path: "links",
          name: "beheer-links",
          component: () => import("../views/beheer/BeheerLinksView.vue"),
          meta: {
            title: "Beheer - Externe links"
          }
        },
        {
          path: "/forbidden",
          name: "forbidden",
          component: () => import("../views/beheer/ForbiddenView.vue"),
          meta: {
            title: "Geen toegang"
          }
        }
      ]
    }
  ]
});

const title = document.title;

router.beforeEach((to, from) => {
  document.title = `${to.meta?.title ? to.meta.title + " | " : ""}${title}`;

  // keep focus if only the query params are different
  if (to.path === from.path) {
    return;
  }

  document.body.setAttribute("tabindex", "-1");
  document.body.focus();
  document.body.removeAttribute("tabindex");
});

export default router;
