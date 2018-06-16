/**
 * A deferrable value.
 *
 * @export
 * @class Deferrable
 * @template T
 */
export class Deferrable<T> {
    /**
     * Gets the promise of this deferrable.
     *
     * @type {Promise<T>}
     * @memberof Deferrable
     */
    readonly promise: Promise<T>;

    /**
     * Creates an instance of Deferrable.
     * @memberof Deferrable
     */
    constructor() {
        this.promise = new Promise<T>((resolve, reject) => {
            this.resolve = resolve;
            this.reject = reject;
        });
    }

    /**
     * Resolves the promise to the indicated value.
     *
     * @param {(T | PromiseLike<T>)} [value] The resolved value.
     * @memberof Deferrable
     */
    resolve(value?: T | PromiseLike<T>): void {
    }

    /**
     * Rejects the promise with the indicated reason.
     *
     * @param {*} [reason] The reason for rejection.
     * @memberof Deferrable
     */
    reject(reason?: any): void {
    }
}
