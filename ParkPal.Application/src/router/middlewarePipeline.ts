import { NavigationGuardNext, RouteLocationNormalized } from 'vue-router'
import { RootState } from '@/store/types'
import { Store } from 'vuex'

function middlewarePipeline (context: { to: RouteLocationNormalized; from: RouteLocationNormalized; next: NavigationGuardNext; store: Store<RootState>}, middleware: Array<any>, index: number) {
    const nextMiddleware = middleware[index]

    if(!nextMiddleware){
        return context.next;
    }

    return (): void => {
        const nextPipeline = middlewarePipeline(
            context, middleware, index + 1
        )

        nextMiddleware({ ...context, next: nextPipeline })

    }
}

export default middlewarePipeline