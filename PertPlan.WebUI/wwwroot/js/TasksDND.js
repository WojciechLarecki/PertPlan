// handle the dragstart
export function dragStart(e) {
    const grabbedRow = e.target.parentNode;
    e.dataTransfer.setData('text/plain', grabbedRow.id);
    const dropZones = document.getElementsByClassName("dropZone");
    Array.from(dropZones).forEach(dz => {
        dz.classList.remove("drpad");
    });
    setTimeout(() => {
        grabbedRow.classList.add("oppacity05");
    }, 0);
}

export function dragEnd(e) {
    const grabbedRow = e.target.parentNode;
    const dropZones = document.getElementsByClassName("dropZone");
    Array.from(dropZones).forEach(dz => {
        dz.classList.add("drpad");
    });
    setTimeout(() => {
        grabbedRow.classList.remove("oppacity05");
    }, 0);
}

export function dragEnter(e) {
    e.preventDefault();
    e.target.classList.add('drag-over');
}

export function dragOver(e) {
    e.preventDefault();
}

export function dragLeave(e) {
    e.target.classList.remove('drag-over');
}

export function drop(e) {
    e.target.classList.remove('drag-over');

    // get the draggable element
    const id = e.dataTransfer.getData('text/plain');
    const draggable = document.getElementById(id);
    const dropZone = draggable.nextElementSibling;
    const row = e.target.parentNode;

    // add it to the drop target
    row.after(draggable);
    draggable.after(dropZone);
}
