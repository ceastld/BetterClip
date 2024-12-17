class MonacoEditorManager {
    constructor() {
        this.monacoEditor = null;
        this.monacoDiffEditor = null;
        this.defaultText = ['function x() {', '\tconsole.log("Hello world!");', '}'].join('\n');
        this.defaultText1 = "function x() {\n\tconsole.log('Hello world!');\n}";
        this.defaultText2 = "function x() {\n\tconsole.log('Hello Monaco!');\n}";
        this.defineTheme();
    }

    createEditor(text = this.defaultText, language = 'javascript') {
        this.monacoEditor = monaco.editor.create(document.getElementById('container'), {
            value: text,
            language: language,
            automaticLayout: true
        });

        this.monacoEditor.onDidChangeModelContent(() => {
            this.webviewPostMessage('textChanged', this.getText());
        });
    }

    createDiffEditor(text1 = this.defaultText1, text2 = this.defaultText2, language = 'javascript') {
        this.monacoDiffEditor = monaco.editor.createDiffEditor(document.getElementById('container'), {
            automaticLayout: true,
            originalEditable: true,
            modifiedEditable: true
        });
        this.monacoDiffEditor.setModel({
            original: monaco.editor.createModel(text1, language),
            modified: monaco.editor.createModel(text2, language)
        });

        this.monacoDiffEditor.getOriginalEditor().onDidChangeModelContent(() => {
            this.webviewPostMessage('originalTextChanged');
        });

        this.monacoDiffEditor.getModifiedEditor().onDidChangeModelContent(() => {
            this.webviewPostMessage('modifiedTextChanged');
        });
    }

    getText() { return this.monacoEditor ? this.monacoEditor.getValue() : null; }
    getOriginalText() { return this.monacoDiffEditor ? this.monacoDiffEditor.getOriginalEditor().getValue() : null; }
    getModifiedText() { return this.monacoDiffEditor ? this.monacoDiffEditor.getModifiedEditor().getValue() : null; }
    setText(text) { if (this.monacoEditor) { this.monacoEditor.setValue(text); } }
    setOriginalText(text) { if (this.monacoDiffEditor) { this.monacoDiffEditor.getOriginalEditor().setValue(text); } }
    setModifiedText(text) { if (this.monacoDiffEditor) { this.monacoDiffEditor.getModifiedEditor().setValue(text); } }
    
    webviewPostMessage(msgType, data = null) {
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage(JSON.stringify(
                {
                    type: msgType,
                    data: data
                }));
        } else {
            console.warn('webview is not available');
        }
    }

    getAllText() {
        return {
            text: this.getText(),
            originalText: this.getOriginalText(),
            modifiedText: this.getModifiedText()
        };
    }

    defineTheme() {
        monaco.editor.defineTheme('unknown', { base: 'vs', inherit: true })
        monaco.editor.defineTheme('light', {
            base: 'vs-dark',
            inherit: true,
            rules: [{ background: 'FFFFFF00' }],
            colors: {
                'editor.background': '#FFFFFF00',
                'minimap.background': '#FFFFFF00'
            }
        });

        monaco.editor.defineTheme('dark', {
            base: 'vs-dark',
            inherit: true,
            rules: [{ background: '1E1E1E' }],
            colors: {
                'editor.background': '#1E1E1E',
                'minimap.background': '#1E1E1E'
            }
        });

        monaco.editor.defineTheme('highContrast', {
            base: 'hc-black',
            inherit: true,
            rules: [{ background: '000000' }],
            colors: {
                'editor.background': '#000000',
                'minimap.background': '#000000'
            }
        });
    }

    setLanguage(language) {
        if (this.monacoEditor) {
            monaco.editor.setModelLanguage(this.monacoEditor.getModel(), language);
        } else if (this.monacoDiffEditor) {
            monaco.editor.setModelLanguage(this.monacoDiffEditor.getOriginalEditor().getModel(), language);
            monaco.editor.setModelLanguage(this.monacoDiffEditor.getModifiedEditor().getModel(), language);
        }
    }

    setTheme(theme) {
        monaco.editor.setTheme(theme);
    }
}

// Usage
const editor = new MonacoEditorManager();
// Uncomment the desired editor to use
editor.createEditor();
// editorManager.createDiffEditor();

// editorManager.setTheme('vs');
editor.setLanguage('javascript');