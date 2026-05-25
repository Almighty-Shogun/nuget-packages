const escapeHtml = (value: string) => value
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#39;');

export const renderInlineCode = (value: string) => escapeHtml(value)
    .replace(/`([^`]+)`/g, '<code>$1</code>');
