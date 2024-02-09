import * as dnd from "./TasksDND.js";
let taskIndex = 0;
const columnsNumber = 7;
function addRow(table) {
    const newRow = table.insertRow(table.rows.length);
    newRow.classList.add("table-row");
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
    const tr = document.createElement("tr");
    const td = document.createElement("td");
    td.setAttribute("colspan", columnsNumber);
    td.classList.add("drpad", "dropZone");
    tr.appendChild(td);

    td.addEventListener('dragenter', dnd.dragEnter)
    td.addEventListener('dragover', dnd.dragOver);
    td.addEventListener('dragleave', dnd.dragLeave);
    td.addEventListener('drop', dnd.drop);

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
    const deleteButton = document.getElementById("deleteTaskButton");

    addButton.addEventListener("click", (event) => {
        const table = document.getElementById("myTable").getElementsByTagName('tbody')[0];
        addRow(table);
        updateDependentTasksInputs(table);
        updateDeleteButtonState(deleteButton, table.rows.length)
    });

    deleteButton.addEventListener("click", (event) => {
        const table = document.getElementById("myTable").getElementsByTagName('tbody')[0];
        deleteRow(table);
        updateDependentTasksInputs(table);
        updateDeleteButtonState(deleteButton, table.rows.length)
    });

    function deleteRow(table) {
        const rowCount = table.rows.length;
        if (rowCount > 0) {
            table.deleteRow(rowCount - 1);
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

