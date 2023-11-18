let taskIndex = 0


function addRow() {
    const table = document.getElementById("myTable").getElementsByTagName('tbody')[0];
    const newRow = table.insertRow(table.rows.length);

    const cell1 = newRow.insertCell(0);
    const cell2 = newRow.insertCell(1);
    const cell3 = newRow.insertCell(2);
    const cell4 = newRow.insertCell(3);
    const cell5 = newRow.insertCell(4);
    const cell6 = newRow.insertCell(5);

    cell1.innerHTML = `<input type="text" class="form-control tableCell" name='[${taskIndex}].Id' readonly value="${taskIndex}">`;
    cell2.innerHTML = `<input type='text' class='form-control tableCell' name='[${taskIndex}].Name'>`;
    cell3.innerHTML = `<input type='number' class='form-control tableCell' name='[${taskIndex}].PositiveFinishTime'>`;
    cell4.innerHTML = `<input type='number' class='form-control tableCell' name='[${taskIndex}].AverageFinishTime'>`;
    cell5.innerHTML = `<input type='number' class='form-control tableCell' name='[${taskIndex}].NegativeFinishTime'>`;
    cell6.innerHTML = `<input type='text' class='form-control tableCell' placeholder='1, 2, 3...' name='[${taskIndex}].DependOnTasks'>`;

    taskIndex++;
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
    finishTimeAverage.innerHTML = "<p>T<sub>śr</sub>";
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