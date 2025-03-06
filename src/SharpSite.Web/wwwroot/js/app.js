(function () {
  window.getSelectedText = function (cssSelector) {
    const editor = document.querySelector(cssSelector);
    if (editor) {
      const start = editor.selectionStart;
      const end = editor.selectionEnd;
      return editor.value.substring(start, end);
    }
    return '';
  };
})();