import { IElementInfo, ElementInfo } from './elementInfo';

/**
 * Provides the contract for types.
 * 
 * @export
 * @interface ITypeInfo
 * @extends {IElementInfo}
 */
export interface ITypeInfo extends IElementInfo {
    /**
     * Gets the type's namespace.
     * 
     * @type {string}
     * @memberof ITypeInfo
     */
    readonly namespace?: string;
}

export class TypeInfo extends ElementInfo {
    /**
     * Gets the type's namespace.
     * 
     * @type {string}
     * @memberof TypeInfo
     */
    readonly namespace?: string;

    /**
     * Creates a new instance of TypeInfo class.
     */
    constructor(info?: ITypeInfo) {
        super(info);
        if (info) {
            this.namespace = info.namespace;
        }
    }
}
