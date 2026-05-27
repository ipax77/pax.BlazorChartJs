// v1.1

window.Prism = window.Prism || {};
Prism.manual = true;
Prism.disableWorkerMessageHandler = true;

function getChartVersion() {
    if (Chart !== undefined) {
        return Chart.version;
    } else {
        return "0.0"
    }
}

function highlightCode() {
    ensureJavaScriptHighlighting();

    Prism.highlightAll();
}

function ensureJavaScriptHighlighting() {
    if (!window.Prism || Prism.languages.javascript || !Prism.languages.clike) {
        return;
    }

    Prism.languages.javascript = Prism.languages.extend('clike', {
        'class-name': [
            Prism.languages.clike['class-name'],
            {
                pattern: /(\b(?:class|extends|implements|instanceof|interface|new)\s+)[A-Za-z_$][\w$]*/,
                lookbehind: true
            }
        ],
        keyword: /\b(?:as|async|await|break|case|catch|class|const|continue|debugger|default|delete|do|else|export|extends|finally|for|from|function|get|if|import|in|instanceof|let|new|null|of|return|set|static|super|switch|this|throw|try|typeof|undefined|var|void|while|with|yield)\b/,
        number: /\b(?:0[xX][\dA-Fa-f]+|0[bB][01]+|0[oO][0-7]+|(?:\d+(?:\.\d*)?|\.\d+)(?:[Ee][+-]?\d+)?)\b/,
        operator: /--|\+\+|&&|\|\||=>|[!=]==?|<<=?|>>>=?|[-+*/%&|^~<>]=?|\?\?=?|\?\.?|\.{3}/,
        function: /#?(?!\d)[A-Za-z_$][\w$]*(?=\s*(?:\.?\s*\(|`))/
    });

    Prism.languages.js = Prism.languages.javascript;
}
