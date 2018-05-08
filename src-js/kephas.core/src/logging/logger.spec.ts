import { LogLevel, Logger } from './logger';
import { expect } from 'chai';
import 'mocha';

class TestLogger extends Logger {
    content: string = '';
    log(level: LogLevel, exception: Error | null, messageFormat: string, ...args: any[]): void {
        if(!exception) {
            this.content = this.content + `${level},${messageFormat}\n`;
        }
    }
}

describe('Fatal logging', () => {
    it('should set log level to fatal', () => {
        let logger = new TestLogger();
        logger.fatal('message');
        const result = logger.content;
        expect(result).to.equal('0,message\n');
    });
});
