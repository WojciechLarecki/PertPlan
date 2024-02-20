import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';

mermaid.initialize({ startOnLoad: true });

document.addEventListener('DOMContentLoaded', () => {
    const downloadToCSVButton = document.getElementById("exportToCSV");
    const downloadToSVGButton = document.getElementById("exportToSVG");
    const showCriticalPathCheckBox = document.getElementById("showCriticalPath");
    const projectDurationInput = document.getElementById("projectDuration");

    downloadToCSVButton.addEventListener("click", exportToCSV);
    downloadToSVGButton.addEventListener("click", exportToSVG);
    downloadToSVGButton.addEventListener("click", exportToSVG);
    showCriticalPathCheckBox.addEventListener("click", (e) => {
        toggleCriticalPathClasses(e);
    });
    projectDurationInput.addEventListener("input", (e) => {
        updateProjectChance(e.target);
    });
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

function toggleCriticalPathClasses(event) {
    if (event.target.checked) {
        const nodes = document.getElementsByClassName("critical-disabled");
        for (const node of nodes) {
            node.classList.remove("critical-disabled");
            node.classList.add("critical");
        }
    }
    else {
        const nodes = document.getElementsByClassName("critical");
        for (const node of nodes) {
            node.classList.add("critical-disabled");
            node.classList.remove("critical");
        }
    }
}
function updateProjectChance(expectedProjectDuationInput) {
    const expectedProjectDuation = Number(expectedProjectDuationInput.value);
    let result = (expectedProjectDuation - criticalPathLength) / projectStandardDeviation
    result = result.toPrecision(3);
    const status = document.getElementById("projectSuccessStatus");

    // clears status
    status.classList.remove("noData", "bg-success", "bg-primary", "bg-warning", "bg-danger", "text-white", "text-black");

    // set new status
    if (expectedProjectDuationInput.value.trim() == '') {
        status.classList.add("noData", "text-black");
        status.textContent = "Brak danych.";
    } else if (result > 0.8) {
        status.classList.add("bg-success", "text-white");
        status.textContent = "Bardzo prawdopodobne.";
    } else if (result > 0) {
        status.classList.add("bg-primary", "text-white");
        status.textContent = "Prawdopodobne.";
    } else if (result > -0.8) {
        status.classList.add("bg-warning", "text-black");
        status.textContent = "Ma³o prawdopodobne.";
    } else {
        status.classList.add("bg-danger", "text-white");
        status.textContent = "Bardzo ma³o prawdopodobne.";
    }
}