import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
mermaid.initialize({ startOnLoad: true });

document.addEventListener('DOMContentLoaded', () => {
    const downloadToCSVButton = document.getElementById("exportToCSV");
    const downloadToSVGButton = document.getElementById("exportToSVG");

    downloadToCSVButton.addEventListener("click", exportToCSV);
    downloadToSVGButton.addEventListener("click", exportToSVG);
});

function exportToCSV() {
    console.log("CSV downloaded");
}

function exportToSVG() {
    const svg = document.querySelector("svg[id|=mermaid]");
    const blob = new Blob([svg.outerHTML], { type: "image/svg+xml" });
    var url = window.URL.createObjectURL(blob);
    var a = document.createElement('a');
    a.href = url;
    a.download = 'moje_obrazek.svg';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
}