import * as dnd from "./TasksDND.js";
let taskIndex = 0;
const columnsNumber = 7;

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
    const deleteLastBtn = document.getElementById("deleteLastTaskButton");
    const deleteSelectedTaskBtn = document.getElementById("deleteSelectedTaskButton");
    const submitButton = document.getElementById("submitButton");

    applyMissingEvents();
    adjustTableState();

    addButton.addEventListener("click", (event) => {
        const table = document.getElementById("myTable").getElementsByTagName('tbody')[0];
        const newRow = createTableRow();
        const dndRow = createDNDTableRow();
        table.appendChild(newRow);
        table.appendChild(dndRow);
        adjustTableState();
        taskIndex++;
    });

    deleteLastBtn.addEventListener("click", (event) => {
        const table = document.getElementById("myTable").getElementsByTagName('tbody')[0];
        deleteLastRow(table);
        adjustTableState();
        taskIndex--;
    });

    deleteSelectedTaskBtn.addEventListener("click", (e) => {
        const table = document.getElementById("myTable").getElementsByTagName('tbody')[0];
        const selectedRow = document.querySelector(".table-active");
        const dndRow = selectedRow.nextElementSibling;
        table.removeChild(selectedRow);
        table.removeChild(dndRow);
        adjustTableState();
        taskIndex--;
    })

    insertBtn.addEventListener("click", (e) => {
        const appendRow = document.querySelector(".table-active").nextElementSibling;
        const newRow = createTableRow();
        const dndRow = createDNDTableRow();
        appendRow.after(newRow);
        newRow.after(dndRow);
        adjustTableState();
        taskIndex++;
    });

    submitButton.addEventListener("click", (e) => {
        const rows = document.getElementsByClassName("table-row");

        for (const row of rows) {
            const taskNumberInput = row.querySelector(".taskNumberInput");
            const taskNameInput = row.querySelector(".taskNameInput");
            const taskPositiveTimeInput = row.querySelector(".taskPositiveTimeInput");
            const taskAverageTimeInput = row.querySelector(".taskAverageTimeInput");
            const taskNegativeTimeInput = row.querySelector(".taskNegativeTimeInput");
            const taskDependOnInput = row.querySelector(".taskDependentTasks");

            validateNameInput(taskNameInput);
            validateAverageTimeInput(taskPositiveTimeInput, taskAverageTimeInput, taskNegativeTimeInput);
            validateNegativeTimeInput(taskNegativeTimeInput, taskAverageTimeInput);
            validateDependentTasksInput(taskDependOnInput, taskNumberInput.textContent);
        }

        const tbody = document.getElementById("table-body");
        localStorage.setItem("tbody", tbody.innerHTML);
    });

    // unselects row 
    const body = document.getElementsByTagName("body")[0];
    body.addEventListener("click", (e) => {
        if (body.isSameNode(e.target)) {
            const activeRow = document.querySelector(".table-active");
            if (activeRow != null) {
                activeRow.classList.remove("table-active");
                updateDeleteSelectedButtonState();
                updateInsertButtonState();
            }
        }
    });

    // populate document if user backs from diagram
    const tbody = document.getElementById("table-body");
    const savedBody = localStorage.getItem("tbody");
    if (savedBody != null) {
        tbody.innerHTML = savedBody;
        localStorage.removeItem("tbody");
    }
});

function deleteLastRow(table) {
    const rowCount = table.rows.length;
    if (rowCount > 0) {
        table.deleteRow(rowCount - 1); // delete place for DND
        table.deleteRow(rowCount - 2); // delete actual row
    }
}

function updateDeleteLastButtonState() {
    const deleteLastBtn = document.getElementById("deleteLastTaskButton");
    const rows = document.getElementsByClassName("table-row");

    if (rows === undefined || deleteLastBtn === undefined) {
        throw new Error("Elements has not been found.");
    }

    if (rows.length === 0) {
        deleteLastBtn.setAttribute("disabled", "disabled");
    } else {
        deleteLastBtn.removeAttribute("disabled");
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

    number.innerHTML = localizer.numberIndex;
    name.innerHTML = localizer.nameIndex;
    finishTime.innerHTML = localizer.finishTimeIndex;
    dependencies.innerHTML = localizer.previousTasksIndex;
    finishTimePositive.innerHTML = localizer.positiveTimeIndex;
    finishTimeAverage.innerHTML = localizer.averageTimeIndex;
    finishTimeNegative.innerHTML = localizer.negativeTimeIndex;
}

function changeText2() {
    const number = document.getElementById("number");
    const name = document.getElementById("name");
    const finishTime = document.getElementById("finishTime");
    const dependencies = document.getElementById("dependencies");
    const finishTimePositive = document.getElementById("finishTimePositive");
    const finishTimeAverage = document.getElementById("finishTimeAverage");
    const finishTimeNegative = document.getElementById("finishTimeNegative");

    number.innerHTML = localizer.number;
    name.innerHTML = localizer.name;
    finishTime.innerHTML = localizer.finishTime;
    dependencies.innerHTML = localizer.procedingTasks;
    finishTimePositive.innerHTML = localizer.positive;
    finishTimeAverage.innerHTML = localizer.average;
    finishTimeNegative.innerHTML = localizer.negative;
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
    cell0.innerHTML = `<div class="threeDotsIcon">^</div>`;

    cell1.innerHTML = `<div type="text" class="form-control tableCell taskNumberInput" readonly>${taskIndex}</div>`;
    cell2.innerHTML = `<input type='text' class='form-control tableCell taskNameInput' name='[${taskIndex}].Name' required maxlength='100'>`;
    cell3.innerHTML = `<input type='number' class='form-control tableCell taskPositiveTimeInput' name='[${taskIndex}].PositiveFinishTime' step="0.5" min="0.5" max="999">`;
    cell4.innerHTML = `<input type='number' class='form-control tableCell taskAverageTimeInput' name='[${taskIndex}].AverageFinishTime' step="0.5" min="0.5" max="999">`;
    cell5.innerHTML = `<input type='number' class='form-control tableCell taskNegativeTimeInput' name='[${taskIndex}].NegativeFinishTime' step="0.5" min="0.5" max='999'>`;
    cell6.innerHTML = `<input type='text' class='form-control tableCell taskDependentTasks' name='[${taskIndex}].DependOnTasks' placeholder="1, 2, 3..." value="x">`;

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
    td.addEventListener('drop', (e) => {
        dnd.drop(e);
        resetRowsNumbers();
        updateRowsFormAttributes();
    });

    return tr;
}

function onClickRow(event, clickedRow) {
    const table = document.getElementById("myTable").getElementsByTagName('tbody')[0];


    // user clicks the same row
    if (clickedRow.contains(event.target) && clickedRow.classList.contains("table-active")) {
        return;
    }

    // user clicks new row
    const rows = table.getElementsByClassName("table-row");
    for (const row of rows) {
        if (!row.isSameNode(event.target) && row.classList.contains("table-active"))
            row.classList.remove("table-active");
    }
    clickedRow.classList.add("table-active");

    updateDeleteSelectedButtonState();
    updateInsertButtonState();
}

function resetRowsNumbers() {
    const numericalValues = document.querySelectorAll(".taskNumberInput");

    if (numericalValues === undefined) {
        throw new Error("Numbers to reset not found.");
    }

    for (var i = 0; i < numericalValues.length; i++) {
        numericalValues[i].textContent = i;
    }
}

function adjustTableState() {
    resetRowsNumbers();
    updateRowsFormAttributes();
    updateDeleteLastButtonState();
    updateDeleteSelectedButtonState();
    updateInsertButtonState();
}

function updateDeleteSelectedButtonState() {
    const selectedRow = document.querySelector(".table-active");
    const deleteSelectedBtn = document.getElementById("deleteSelectedTaskButton");

    if (deleteSelectedBtn == null) {
        throw Error("Element not found.");
    }

    if (selectedRow == null) {
        deleteSelectedBtn.setAttribute("disabled", "true");
    } else {
        deleteSelectedBtn.removeAttribute("disabled");
    }
}

function updateInsertButtonState() {
    const selectedRow = document.querySelector(".table-active");
    const insertBtn = document.getElementById("insertTaskButton");

    if (insertBtn == null) {
        throw Error("Element not found.");
    }

    if (selectedRow == null) {
        insertBtn.setAttribute("disabled", "true");
    } else {
        insertBtn.removeAttribute("disabled");
    }
}

function updateRowsFormAttributes() {
    const rows = document.getElementsByClassName("table-row");

    if (rows == null) {
        throw Error("Elements not found.");
    }

    const rowsArray = Array.from(rows);

    for (var i = 0; i < rowsArray.length; i++) {
        const row = rows[i];
        const taskName = row.querySelector(".taskNameInput");
        const taskPositiveFinishTime = row.querySelector(".taskPositiveTimeInput");
        const taskAverageFinishTime = row.querySelector(".taskAverageTimeInput");
        const taskNegativeFinishTime = row.querySelector(".taskNegativeTimeInput");
        const taskDependOnTasks = row.querySelector(".taskDependentTasks");

        if (taskName == null || taskPositiveFinishTime == null ||
            taskAverageFinishTime == null || taskNegativeFinishTime == null ||
            taskDependOnTasks == null) {
            throw Error(`Element not found in row ${i}`);
        }

        taskName.setAttribute("name", `[${i}].Name`);
        taskPositiveFinishTime.setAttribute("name", `[${i}].PositiveFinishTime`);
        taskAverageFinishTime.setAttribute("name", `[${i}].AverageFinishTime`);
        taskNegativeFinishTime.setAttribute("name", `[${i}].NegativeFinishTime`);
        taskDependOnTasks.setAttribute("name", `[${i}].DependOnTasks`);
    }
}

function isNumber(str) {
    return !isNaN(str);
}

function validateNameInput(nameInput) {
    if (nameInput.value.trim() === '') {
        nameInput.setCustomValidity(localizer.blankSpacesError);
    } else {
        nameInput.setCustomValidity("");
    }
}

function validateAverageTimeInput(positiveTimeInput, averageTimeInput, negativeTimeInput) {
    if (Number(averageTimeInput.value) < Number(positiveTimeInput.value)) {
        averageTimeInput.setCustomValidity(localizer.averageTimeTooShortError);
    } else if (Number(averageTimeInput.value) > Number(negativeTimeInput.value)) {
        averageTimeInput.setCustomValidity(localizer.averageTimeTooLongError);
    } else {
        averageTimeInput.setCustomValidity("");
    }
}

function validateNegativeTimeInput(negativeTimeInput, averageTimeInput) {
    if (Number(negativeTimeInput.value) < Number(averageTimeInput.value)) {
        negativeTimeInput.setCustomValidity(localizer.negativeTimeError);
    } else {
        negativeTimeInput.setCustomValidity("");
    }
}

function validateDependentTasksInput(dependentTasksInput, strTaskNumber) {
    const dependTasksNumbers = dependentTasksInput.value.split(',');

    if (!isNumber(strTaskNumber)) {
        throw Error(`Given task number is not a number.`);
    }

    const taskNumber = Number(strTaskNumber);

    for (let numberStr of dependTasksNumbers) {
        numberStr = numberStr.trim().toLowerCase();

        const strIsNumber = isNumber(numberStr);

        if (numberStr === "") {
            dependentTasksInput.setCustomValidity(localizer.emptyProceedingTaskField);
            break;
        } else if (numberStr !== "x" && !strIsNumber) {
            dependentTasksInput.setCustomValidity(localizer.dependencyFormatError);
            break;
        }

        const number = Number(numberStr);

        if (number < 0) {
            dependentTasksInput.setCustomValidity(localizer.negativeNumberError);
        } else if (number > taskNumber) {
            dependentTasksInput.setCustomValidity(localizer.undefinedTaskError);
        } else if (number === taskNumber) {
            dependentTasksInput.setCustomValidity(localizer.selfDependencyError);
        } else {
            dependentTasksInput.setCustomValidity("");
        }
    }
}

function applyMissingEvents() {
    const tableRows = document.getElementsByClassName("table-row");
    const tds = document.getElementsByClassName("dropZone"); // table data elements of DND rows

    for (const row of tableRows) {
        row.addEventListener("click", (e) => onClickRow(e, row));
        row.addEventListener('dragstart', dnd.dragStart);
        row.addEventListener('dragend', dnd.dragEnd);
    }

    for (const td of tds) {
        td.addEventListener('dragenter', dnd.dragEnter)
        td.addEventListener('dragover', dnd.dragOver);
        td.addEventListener('dragleave', dnd.dragLeave);
        td.addEventListener('drop', (e) => {
            dnd.drop(e);
            resetRowsNumbers();
            updateRowsFormAttributes();
        });
    }
}