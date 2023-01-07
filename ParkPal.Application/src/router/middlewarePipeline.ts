import PipelineContext from '@/models/router/PipelineContext';

function middlewarePipeline(context: PipelineContext, middleware: Array<(context: PipelineContext) => void | undefined>, index: number) {
    const nextMiddleware = middleware[index]

    if(!nextMiddleware){
        return context.next;
    }

    return (): void => {
        const nextPipeline = middlewarePipeline(
            context, middleware, index + 1
        )

        if(nextMiddleware) {
            nextMiddleware({...context, next: nextPipeline})
        }

    }
}

export default middlewarePipeline