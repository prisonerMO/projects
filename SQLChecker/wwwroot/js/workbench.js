document.addEventListener("DOMContentLoaded", () => {

    console.log("[WB] script loaded");

    const resizeHandle = document.getElementById('resize-handle');
    const editorArea = document.querySelector('.wb-editor-area');
    const resultsArea = document.querySelector('.wb-results-area');
    const main = document.querySelector('.wb-main');

    console.log({
        resizeHandle,
        editorArea,
        resultsArea,
        main
    });

    if (!resizeHandle || !editorArea || !main) {
        console.error("[WB] Missing required elements - resize disabled");
        return;
    }

    let isResizing = false;

    resizeHandle.addEventListener('mousedown', (e) => {
        console.log("[WB] mousedown resize");

        isResizing = true;

        document.body.style.cursor = 'row-resize';
        document.body.style.userSelect = 'none';

        e.preventDefault();
    });

    document.addEventListener('mousemove', (e) => {

        if (!isResizing) return;

        const mainRect = main.getBoundingClientRect();

        const newEditorHeight = e.clientY - mainRect.top;

        const minEditorHeight = 150;
        const minResultsHeight = 120;

        const maxEditorHeight =
            mainRect.height - minResultsHeight - resizeHandle.offsetHeight;

        console.log("[WB] move", {
            y: e.clientY,
            newEditorHeight,
            maxEditorHeight
        });

        if (
            newEditorHeight >= minEditorHeight &&
            newEditorHeight <= maxEditorHeight
        ) {
            editorArea.style.height = `${newEditorHeight}px`;

            console.log("[WB] applied height:", editorArea.style.height);
        }
    });

    document.addEventListener('mouseup', () => {

        if (isResizing) {
            console.log("[WB] mouseup stop resize");
        }

        isResizing = false;

        document.body.style.cursor = '';
        document.body.style.userSelect = '';
    });

});

function toggleWeek(el) {
    const list = el.nextElementSibling;
    const isOpen = list.classList.contains('open');
    list.classList.toggle('open', !isOpen);
    el.classList.toggle('open', !isOpen);
    el.querySelector('.wb-chevron').textContent = isOpen ? '▸' : '▾';
}

function toggleSchema(el) {
    const cols = el.nextElementSibling;
    const isOpen = cols.style.display !== 'none';
    cols.style.display = isOpen ? 'none' : 'block';
    el.querySelector('span').textContent = isOpen ? '▸' : '▾';
}

function selectTask(el, name, desc, taskId) {
    document.querySelectorAll('.wb-task-item').forEach(t => t.classList.remove('active'));
    el.classList.add('active');
    document.getElementById('task-title').textContent = name;
    document.getElementById('task-desc').textContent = desc;
    document.getElementById('task-id').value = taskId;
    clearEditor();
    document.getElementById('results-container').innerHTML =
        '<div class="wb-results-empty">Aja kysely nähdäksesi tulokset</div>';
    document.getElementById('results-meta').textContent = '';
}

function clearEditor() {
    document.getElementById('sql-editor').value = '';
}

async function runQuery() {
    const sql = document.getElementById('sql-editor').value.trim();
    const dbName = document.getElementById('database-name').value;
    const btn = document.querySelector('.btn-wb-run');
    console.log("DATABASE: ", dbName);
    if (!sql) return;

    btn.textContent = 'Ajetaan...';
    btn.disabled = true;
    document.getElementById('results-meta').textContent = '';

    const start = performance.now();

    try {
        const response = await fetch('/Workbench/RunQuery', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ sql, databaseName: dbName })
        });

        const elapsed = Math.round(performance.now() - start);
        const html = await response.text();

        document.getElementById('results-container').innerHTML = html;
        const wrapper = document.querySelector('#results-container #query-result');
        const rowCount = wrapper?.dataset.rowcount;
        if (rowCount) {
            document.getElementById('results-meta').textContent =
                `${rowCount} riviä palautettu | ${elapsed}ms`;
        }

    } catch (err) {
        document.getElementById('results-container').innerHTML =
            `<div class="wb-results-error">Verkkovirhe: ${err.message}</div>`;
    } finally {
        btn.textContent = '▶ Aja kysely';
        btn.disabled = false;
    }
}
document.getElementById('sql-editor').addEventListener('keydown', function (e) {
    if (e.ctrlKey && e.key === 'Enter') runQuery();
});