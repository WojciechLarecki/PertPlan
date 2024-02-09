import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
mermaid.initialize({ startOnLoad: true });

document.addEventListener('DOMContentLoaded', () => {
    const downloadToCSVButton = document.getElementById("exportToCSV");
    const downloadToSVGButton = document.getElementById("exportToSVG");

    downloadToCSVButton.addEventListener("click", exportToCSV);
    downloadToSVGButton.addEventListener("click", exportToSVG);
});

function exportToCSV() {
    const blob = new Blob([csvContent], { type: "text" });
    var url = window.URL.createObjectURL(blob);
    var a = document.createElement('a');
    a.href = url;
    a.download = 'tasks.csv';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
}

function exportToSVG() {
    const svg = document.querySelector("svg[id|=mermaid]");
    const blob = new Blob([svg.outerHTML], { type: "image/svg+xml" });
    var url = window.URL.createObjectURL(blob);
    var a = document.createElement('a');
    a.href = url;
    a.download = 'graph.svg';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
}