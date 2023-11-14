
function getKeyByValue(object, value) {
    return Object.keys(object).find(key => object[key] === value);
}
function setRealtimeChart(element_id, label_element_id, max, min, interval, data_info) {
    Chart.defaults.global.defaultFontFamily = '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
    Chart.defaults.global.defaultFontColor = '#292b2c';

    /*var labels = [" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "]*/;
    var labels = Array(120).fill(" ");
    var data;

    var ctx = document.getElementById(element_id);
    Datasets = []
    //label, data_element_ids, label_element_ids, colors, border_colors, mins, maxs
    for (var i = 0; i < data_info.length; i++) {
        data = Array(120).fill(0)
        Datasets.push({
            label: data_info[i].label,
            data: data,
            lineTension: 0.3,
            type: 'line',
            backgroundColor: data_info[i].color,
            borderColor: data_info[i].border_color,
            pointBackgroundColor: "rgba(0,0,0,0)",
            pointBorderColor: "rgba(0,0,0,0)",
            pointHoverBackgroundColor: "rgba(0,0,0,0)",
        })
    }
    var myLineChart = new Chart(ctx, {
        data: {
            labels: labels,
            datasets: Datasets,
            spanGaps: true
        },
        options: {
            responsive: true,
            //maintainAspectRatio: false,
            scales: {
                xAxes: [{
                    time: {
                        unit: 'date'
                    },
                    gridLines: {
                        display: true
                    },
                    ticks: {
                        maxTicksLimit: 7
                    }
                }],
                yAxes: [{
                    ticks: {
                        min: min,
                        max: max,
                        maxTicksLimit: 5
                    },
                    gridLines: {
                        color: "rgba(0, 0, 0, .125)",
                    }
                }],
            },
            legend: {
                display: true
            }
        }
    });

    myLineChart.update('none');
    $(document).ready(function () {
        setInterval(function () {
            var label = $("#" + label_element_id).html();
            myLineChart.data.labels.shift();
            myLineChart.data.labels.push(label);

            myLineChart.data.datasets.forEach((dataset) => {
                for (var i = 0; i < data_info.length; i++) {
                    if (dataset.label == data_info[i].label) {
                        var data = $("#" + data_info[i].data_element_id).html();
                        //alert(data_info[i].label + "\n" + dataset.label + "\n" + data_info[i].data_element_id + "\n" + data);
                        dataset.data.shift();
                        dataset.data.push(data);
                    }
                }
            });
            myLineChart.update();
        }, interval);
    });
}
