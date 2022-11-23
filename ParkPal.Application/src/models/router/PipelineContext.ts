import {NavigationGuardNext, RouteLocationNormalized} from "vue-router";
import {Store} from "vuex";
import {RootState} from "@/store/types";

export default class PipelineContext {
    to?: RouteLocationNormalized;
    from?: RouteLocationNormalized;
    next?: NavigationGuardNext;
    store?: Store<RootState>
}