import * as dnd from "./TasksDND.js";
let taskIndex = 0;
const columnsNumber = 7;
function appendRow(table) {
    const newRow = table.insertRow(table.rows.length);
    newRow.classList.add("table-row");
    newRow.addEventListener("click", (e) => onClickRow(e, newRow));

    const addingFirstRow = taskIndex === 0;

    newRow.id = (Math.random() * 1000000).toFixed();
    newRow.addEventListener('dragstart', dnd.dragStart);
    newRow.addEventListener('dragend', dnd.dragEnd);

    const cell0 = newRow.insertCell(0);
    const cell1 = newRow.insertCell(1);
    const cell2 = newRow.insertCell(2);
    const cell3 = newRow.insertCell(3);
    const cell4 = newRow.insertCell(4);
    const cell5 = newRow.insertCell(5);
    const cell6 = newRow.insertCell(6);

    if (!addingFirstRow) {
        cell0.setAttribute("draggable", true);
        cell0.classList.add("dndCell");
        cell0.innerHTML = `<div class="threeDotsIcon">
                            <i class="bi bi-three-dots-vertical"></i>
                           </div>`;
        cell0.addEventListener("mouseenter", onMouseEnterThreeDots)
        cell0.addEventListener("mouseleave", onMouseLeaveThreeDots)
    }

    cell1.innerHTML = `<input type="text" class='form-control tableCell taskNumberInput'
                        name='[${taskIndex}].Id' readonly value="${taskIndex}">`;
    cell2.innerHTML = `<input type='text' class='form-control tableCell taskNameInput'
                        name='[${taskIndex}].Name'>`;
    cell3.innerHTML = `<input type='number' class='form-control tableCell taskPositiveTimeInput'
                        name='[${taskIndex}].PositiveFinishTime'>`;
    cell4.innerHTML = `<input type='number' class='form-control tableCell taskAverageTimeInput'
                        name='[${taskIndex}].AverageFinishTime'>`;
    cell5.innerHTML = `<input type='number' class='form-control tableCell taskNegativeInput'
                        name='[${taskIndex}].NegativeFinishTime'>`;
    cell6.innerHTML = `<input type='text' class='form-control tableCell taskDependentTasks'
                        name='[${taskIndex}].DependOnTasks'>`;

    taskIndex++;
    const tr = createDNDTableRow();
    newRow.after(tr);
}

window.addEventListener("resize", () => {
    // outher width creates errors
    if (window.innerWidth < 768) {
        changeText();
    }
    else {
        changeText2();
    }
});

document.addEventListener("DOMContentLoaded", () => {
    const addButton = document.getElementById("addTaskButton");
    const insertBtn = document.getElementById("insertTaskButton");
    const deleteLastBtn = document.getElementById("deleteTaskButton");
    const deleteSelectedTaskBtn = document.getElementById("deleteSelectedTaskButton");

    addButton.addEventListener("click", (event) => {
        const table = document.getElementById("myTable").getElementsByTagName('tbody')[0];
        appendRow(table);
        updateDependentTasksInputs(table);
        updateDeleteButtonState(deleteLastBtn, table.rows.length)
    });

    deleteLastBtn.addEventListener("click", (event) => {
        const table = document.getElementById("myTable").getElementsByTagName('tbody')[0];
        deleteLastRow(table);
        updateDependentTasksInputs(table);
        updateDeleteButtonState(deleteLastBtn, table.rows.length)
    });

    deleteSelectedTaskBtn.addEventListener("click", (e) => {
        const table = document.getElementById("myTable").getElementsByTagName('tbody')[0];
        const selectedRow = document.querySelector(".table-active");
        const dndRow = selectedRow.nextElementSibling;
        table.removeChild(selectedRow);
        table.removeChild(dndRow);
    })

    insertBtn.addEventListener("click", (e) => {
        const appendRow = document.querySelector(".table-active").nextElementSibling;
        const newRow = createTableRow();
        const dndRow = createDNDTableRow();
        appendRow.after(newRow);
        newRow.after(dndRow);
        taskIndex++;
    });

    function deleteLastRow(table) {
        const rowCount = table.rows.length;
        if (rowCount > 0) {
            table.deleteRow(rowCount - 1); // delete place for DND
            table.deleteRow(rowCount - 2); // delete actual row
            taskIndex--; // Decrease the taskIndex when a row is deleted
        }
    }

    function updateDependentTasksInputs(table) {
        const dependentTasksInputs = table.getElementsByClassName("taskDependentTasks");

        for (let input of dependentTasksInputs) {
            if (input.isSameNode(dependentTasksInputs[0])) {
                input.setAttribute("disabled", "disabled");
                input.setAttribute("title", "To pierwsze zadanie projektu.\nNie posiada zadań nadrzędnych.")
            } else {
                input.removeAttribute("disabled");
                input.setAttribute("placeholder", "1, 2, 3...");
            }
        }
    }
});

function updateDeleteButtonState(button, rowCount) {
    if (rowCount === 0) {
        button.setAttribute("disabled", "disabled");
    } else {
        button.removeAttribute("disabled");
    }
}


function changeText() {
    const number = document.getElementById("number");
    const name = document.getElementById("name");
    const finishTime = document.getElementById("finishTime");
    const dependencies = document.getElementById("dependencies");
    const finishTimePositive = document.getElementById("finishTimePositive");
    const finishTimeAverage = document.getElementById("finishTimeAverage");
    const finishTimeNegative = document.getElementById("finishTimeNegative");

    number.innerHTML = "<p>N<sub>um</sub>";
    name.innerHTML = "<p>N<sub>az</sub>";
    finishTime.innerHTML = "<p>T<sub>e</sub>";
    dependencies.innerHTML = "D";
    finishTimePositive.innerHTML = "<p>T<sub>p</sub>";
    finishTimeAverage.innerHTML = "<p>T<sub>sr</sub>";
    finishTimeNegative.innerHTML = "<p>T<sub>n</sub>";
}

function changeText2() {
    const number = document.getElementById("number");
    const name = document.getElementById("name");
    const finishTime = document.getElementById("finishTime");
    const dependencies = document.getElementById("dependencies");
    const finishTimePositive = document.getElementById("finishTimePositive");
    const finishTimeAverage = document.getElementById("finishTimeAverage");
    const finishTimeNegative = document.getElementById("finishTimeNegative");

    number.innerHTML = "Numer";
    name.innerHTML = "Nazwa";
    finishTime.innerHTML = "Czas ukończenia";
    dependencies.innerHTML = "Poprzedzające zadania";
    finishTimePositive.innerHTML = "Pozytywny";
    finishTimeAverage.innerHTML = "Średni";
    finishTimeNegative.innerHTML = "Negatywny";
}

function onMouseEnterThreeDots(e) {
    //const row = e.target.parentNode;
    //row.classList.add("table-active");
    //row.classList.add("table-active");
}

function onMouseLeaveThreeDots(e) {
    //const row = e.target.parentNode;
    //row.classList.remove("table-active");
    //row.classList.remove("table-active");

}

function createTableRow() {
    const newRow = document.createElement("tr");
    newRow.classList.add("table-row");
    newRow.addEventListener("click", (e) => onClickRow(e, newRow));

    newRow.id = (Math.random() * 1000000).toFixed();
    newRow.addEventListener('dragstart', dnd.dragStart);
    newRow.addEventListener('dragend', dnd.dragEnd);

    const cell0 = newRow.insertCell(0);
    const cell1 = newRow.insertCell(1);
    const cell2 = newRow.insertCell(2);
    const cell3 = newRow.insertCell(3);
    const cell4 = newRow.insertCell(4);
    const cell5 = newRow.insertCell(5);
    const cell6 = newRow.insertCell(6);

    cell0.setAttribute("draggable", true);
    cell0.classList.add("dndCell");
    cell0.innerHTML = `<div class="threeDotsIcon">
                            <i class="bi bi-three-dots-vertical"></i>
                           </div>`;
    cell0.addEventListener("mouseenter", onMouseEnterThreeDots)
    cell0.addEventListener("mouseleave", onMouseLeaveThreeDots)

    cell1.innerHTML = `<input type="text" class='form-control tableCell taskNumberInput'
                        name='[${taskIndex}].Id' readonly value="${taskIndex}">`;
    cell2.innerHTML = `<input type='text' class='form-control tableCell taskNameInput'
                        name='[${taskIndex}].Name'>`;
    cell3.innerHTML = `<input type='number' class='form-control tableCell taskPositiveTimeInput'
                        name='[${taskIndex}].PositiveFinishTime'>`;
    cell4.innerHTML = `<input type='number' class='form-control tableCell taskAverageTimeInput'
                        name='[${taskIndex}].AverageFinishTime'>`;
    cell5.innerHTML = `<input type='number' class='form-control tableCell taskNegativeInput'
                        name='[${taskIndex}].NegativeFinishTime'>`;
    cell6.innerHTML = `<input type='text' class='form-control tableCell taskDependentTasks'
                        name='[${taskIndex}].DependOnTasks'>`;

    return newRow;
}

function createDNDTableRow() {
    const tr = document.createElement("tr");
    const td = document.createElement("td");
    td.setAttribute("colspan", columnsNumber);
    td.classList.add("drpad", "dropZone");
    tr.appendChild(td);

    td.addEventListener('dragenter', dnd.dragEnter)
    td.addEventListener('dragover', dnd.dragOver);
    td.addEventListener('dragleave', dnd.dragLeave);
    td.addEventListener('drop', dnd.drop);

    return tr;
}

function onClickRow(event, clickedRow) {
    const deleteSelectedRowBtn = document.getElementById("deleteSelectedTaskButton");
    const table = document.getElementById("myTable").getElementsByTagName('tbody')[0];


    // user unclicks row
    if (clickedRow.contains(event.target) && clickedRow.classList.contains("table-active")) {
        clickedRow.classList.remove("table-active");
        deleteSelectedRowBtn.setAttribute("disabled", "true");
        return;
    }

    // user clicks new row
    const rows = table.getElementsByClassName("table-row");
    for (const row of rows) {
        if (!row.isSameNode(event.target) && row.classList.contains("table-active"))
            row.classList.remove("table-active");
    }
    clickedRow.classList.add("table-active");
    deleteSelectedRowBtn.removeAttribute("disabled");
}
