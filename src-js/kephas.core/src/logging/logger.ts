/**
 * Enumerates the logging levels.
 * 
 * @export
 * @enum {number}
 */
export enum LogLevel {
    /**
     * Fatal errors.
     */
    Fatal,

    /**
     * Common errors.
     */
    Error,

    /**
     * Warning information.
     */
    Warning,

    /**
     * Common information.
     */
    Info,

    /**
     * Debugging information.
     */
    Debug,

    /**
     * Tracing information.
     */
    Trace,
}

/**
 * Base service for logging.
 * 
 * @export
 * @abstract
 * @class Logger
 */
export abstract class Logger {
    /**
     * Logs the information at the provided level.
     * 
     * @abstract
     * @param {LogLevel} level The logging level.
     * @param {Error} exception The error that occured (may not be specified).
     * @param {string} messageFormat The message format.
     * @param {...any[]} args The arguments for the message format.
     * @memberof Logger
     */
    abstract log(level: LogLevel, exception: Error | null, messageFormat: string, ...args: any[]): void;

    /**
     * Logs the event at the fatal level.
     * 
     * @param {Error | string} event The event to be logged.
     * @param {...any[]} args The arguments for the event.
     * @memberof Logger
     */
    fatal(event: Error | string, ...args: any[]): void {
        this._log(LogLevel.Fatal, event, args);
    }

    /**
     * Logs the event at the error level.
     * 
     * @param {Error | string} event The event to be logged.
     * @param {...any[]} args The arguments for the event.
     * @memberof Logger
     */
    error(event: Error | string, ...args: any[]): void {
        this._log(LogLevel.Error, event, args);
    }

    /**
     * Logs the event at the warning level.
     * 
     * @param {Error | string} event The event to be logged.
     * @param {...any[]} args The arguments for the event.
     * @memberof Logger
     */
    warn(event: Error | string, ...args: any[]): void {
        this._log(LogLevel.Warning, event, args);
    }

    /**
     * Logs the event at the information level.
     * 
     * @param {Error | string} event The event to be logged.
     * @param {...any[]} args The arguments for the event.
     * @memberof Logger
     */
    info(event: Error | string, ...args: any[]): void {
        this._log(LogLevel.Info, event, args);
    }

    /**
     * Logs the event at the debug level.
     * 
     * @param {Error | string} event The event to be logged.
     * @param {...any[]} args The arguments for the event.
     * @memberof Logger
     */
    debug(event: Error | string, ...args: any[]): void {
        this._log(LogLevel.Debug, event, args);
    }

    /**
     * Logs the event at the trace level.
     * 
     * @param {Error | string} event The event to be logged.
     * @param {...any[]} args The arguments for the event.
     * @memberof Logger
     */
    trace(event: Error | string, ...args: any[]): void {
        this._log(LogLevel.Trace, event, args);
    }

    private _log(level: LogLevel, event: Error | string, args: any[]): void {
        if (typeof event === 'string') {
            this.log(level, null, event, ...args);
        } else {
            let messageFormat = args && args.length && args[0];
            args = (args && args.length && args.splice(0, 1)) || [];
            this.log(level, event, messageFormat, ...args);
        }
    }
}