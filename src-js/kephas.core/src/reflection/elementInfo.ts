/**
 * Provides basic information about reflection elements.
 * 
 * @export
 * @interface IElementInfo
 */
export interface IElementInfo {
    /**
     * Gets the element name.
     * 
     * @type {string}
     * @memberof IElementInfo
     */
    readonly name: string;

    /**
     * Gets the element full name.
     * 
     * @type {string}
     * @memberof IElementInfo
     */
    readonly fullName?: string;
}

/**
 * Provides basic implementation of reflection elements.
 * 
 * @export
 * @class ElementInfo
 * @implements {IElementInfo}
 */
export abstract class ElementInfo implements IElementInfo {
    /**
     * Gets the element name.
     * 
     * @type {string}
     * @memberof IElementInfo
     */
    readonly name: string = '';

    /**
     * Gets the element full name.
     * 
     * @type {string}
     * @memberof ElementInfo
     */
    readonly fullName?: string;

    /**
     * Initializes a new instance of ElementInfo class.
     */
    constructor(info?: IElementInfo) {
        if (info) {
            this.name = info.name;
            this.fullName = info.fullName;
        }
    }
}
