import { Logger } from "tslog";
import * as Sentry from "@sentry/react";

type LogLevel = "silly" | "trace" | "debug" | "info" | "warn" | "error" | "fatal";

interface LogContext {
    [key: string]: unknown;
}

const tsLogger = new Logger({
    name: "shop-frontend",
    type: "pretty",          // красиво в консоли браузера
    minLevel: 0,       // порог логирования    
});

function log(
    level: LogLevel,
    message: string,
    context?: LogContext,
    error?: unknown
) {
    // 1. Лог в консоль через tslog
    const payload = { message, ...context, error };
    switch (level) {
        case "silly":
        case "trace":
        case "debug":
            tsLogger.debug(payload);
            break;
        case "info":
            tsLogger.info(payload);
            break;
        case "warn":
            tsLogger.warn(payload);
            break;
        case "error":
        case "fatal":
            tsLogger.error(payload);
            break;
    }

    // 2. Отправка ошибок в Sentry
    if (level === "error" || level === "fatal") {
        Sentry.withScope((scope) => {
            if (context) {
                Object.entries(context).forEach(([key, value] : [string, any]) => {
                    scope.setExtra(key, value);
                });
            }

            scope.setLevel(level === "fatal" ? "fatal" : "error");

            if (error instanceof Error) {
                Sentry.captureException(error);
            } else {
                Sentry.captureMessage(message);
            }
        });
    }
}

export const logger = {
    debug: (msg: string, context?: LogContext)=> log("debug", msg, context),
    info: (msg: string, context?: LogContext) => log("info", msg, context),
    warn: (msg: string, context?: LogContext) => log("warn", msg, context),
    error: (msg: string, 
            error?: unknown, 
            context?: LogContext) => log("error", msg, context, error),
    fatal: (msg: string, 
            error?: unknown, 
            context?: LogContext) => log("fatal", msg, context, error),
};
