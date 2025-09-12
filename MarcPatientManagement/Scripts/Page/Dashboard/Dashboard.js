window.onload = function () {

    var sortedDrugData = window.sortedDrugData;
    var patientColors = [
        'rgba(100, 149, 237, 0.8)', 
        'rgba(60, 179, 113, 0.8)',  
        'rgba(255, 160, 180, 0.8)', 
        'rgba(255, 200, 120, 0.8)',
        'rgba(186, 85, 211, 0.8)',  
        'rgba(238, 232, 170, 0.8)'  
    ];

    // Patient Bar Chart
    var ctxPatient = document.getElementById('patientBarChart').getContext('2d');
    new Chart(ctxPatient, {
        type: 'bar',
        data: {
            labels: window.patientLabels,
            datasets: [{
                label: 'Number of Prescriptions',
                data: window.patientCounts,
                backgroundColor: patientColors,
                borderColor: '#fff',
                borderWidth: 1,
                barPercentage: 0.7,
                categoryPercentage: 0.7
            }]
        },
        options: {
            responsive: true,
            plugins: { legend: { display: false } },
            scales: { y: { beginAtZero: true } }
        }
    });

    // Drug Line Chart
    var ctxDrug = document.getElementById('drugLineChart').getContext('2d');
    new Chart(ctxDrug, {
        type: 'line',
        data: {
            labels: sortedDrugData.map(d => d.Name),
            datasets: [{
                label: 'Number of Prescriptions',
                data: sortedDrugData.map(d => d.Count),
                fill: true,
                backgroundColor: 'rgba(255, 206, 86, 0.2)',
                borderColor: 'rgba(255, 99, 132, 1)',
                tension: 0.3,
                pointBackgroundColor: 'rgba(255,99,132,1)',
                pointRadius: 5
            }]
        },
        options: {
            responsive: true,
            plugins: { legend: { display: false } },
            scales: { y: { beginAtZero: true } }
        }
    });
};
