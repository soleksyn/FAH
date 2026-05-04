/**
 * charts.js — Chart.js initialization helpers for SportMatrix dashboard
 *
 * Called from Dashboard/Index.cshtml after data is serialized into
 * window.chartData by Razor.
 */

(function () {
    'use strict';

    const ORANGE  = '#F97316';
    const ORANGE2 = '#FB923C';
    const ORANGE3 = '#FDBA74';
    const MINT    = '#10B981';
    const MINT2   = '#34D399';
    const MINT3   = '#6EE7B7';
    const GRAY    = '#A8A29E';

    const SPORT_COLOURS = {
        'Run':     ORANGE,
        'Running': ORANGE,
        'Ride':    ORANGE2,
        'Cycling': ORANGE2,
        'Swim':    MINT,
        'Swimming':MINT,
        'Walk':    MINT2,
        'Walking': MINT2,
        'Hike':    ORANGE3,
        'Hiking':  ORANGE3,
        'Workout': '#EF4444',
        'Yoga':    '#8B5CF6',
    };

    function sportColour(name) {
        if (!name) return GRAY;
        const key = Object.keys(SPORT_COLOURS).find(k => name.toLowerCase().includes(k.toLowerCase()));
        return key ? SPORT_COLOURS[key] : GRAY;
    }

    const defaultFont = {
        family: "'Inter', system-ui, sans-serif",
        size: 12,
    };

    Chart.defaults.font = defaultFont;
    Chart.defaults.color = '#78716C';

    /* ── Weekly progress bar chart ─────────────────────────── */
    window.initWeeklyChart = function (canvasId, weeks) {
        const ctx = document.getElementById(canvasId);
        if (!ctx || !weeks || !weeks.length) return;

        const labels   = weeks.map((_, i) => `Week ${weeks.length - i}`);
        const distances = weeks.map(w => Math.round(w.distance * 10) / 10);
        const maxDist  = Math.max(...distances, 1);

        new Chart(ctx, {
            type: 'bar',
            data: {
                labels,
                datasets: [{
                    label: 'Distance (km)',
                    data: distances,
                    backgroundColor: weeks.map((_, i) => {
                        const alpha = 0.55 + (i / weeks.length) * 0.45;
                        return `rgba(249,115,22,${alpha.toFixed(2)})`;
                    }),
                    borderColor: ORANGE,
                    borderWidth: 0,
                    borderRadius: 6,
                    borderSkipped: false,
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: ctx => `${ctx.parsed.y} km`
                        }
                    }
                },
                scales: {
                    x: { grid: { display: false }, border: { display: false } },
                    y: {
                        grid: { color: '#EDE8E3' },
                        border: { display: false, dash: [4, 4] },
                        ticks: { callback: v => v + ' km' },
                        max: Math.ceil(maxDist * 1.15),
                        beginAtZero: true,
                    }
                }
            }
        });
    };

    /* ── Activity distribution doughnut ────────────────────── */
    window.initDoughnutChart = function (canvasId, types) {
        const ctx = document.getElementById(canvasId);
        if (!ctx || !types || !types.length) return;

        const active = types.filter(t => t.count > 0);
        if (!active.length) return;

        new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: active.map(t => t.name),
                datasets: [{
                    data: active.map(t => t.count),
                    backgroundColor: active.map(t => sportColour(t.name)),
                    borderColor: '#FFFFFF',
                    borderWidth: 3,
                    hoverBorderWidth: 2,
                    hoverOffset: 6,
                }]
            },
            options: {
                responsive: true,
                cutout: '65%',
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 14,
                            boxWidth: 12,
                            boxHeight: 12,
                            borderRadius: 3,
                            usePointStyle: true,
                            pointStyle: 'rectRounded',
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: ctx => ` ${ctx.label}: ${ctx.parsed} (${ctx.dataset.data.reduce((a, b) => a + b, 0) > 0 ? Math.round(ctx.parsed / ctx.dataset.data.reduce((a, b) => a + b, 0) * 100) : 0}%)`
                        }
                    }
                }
            }
        });
    };

    /* ── Monthly heatmap bar chart ─────────────────────────── */
    window.initMonthlyChart = function (canvasId, months) {
        const ctx = document.getElementById(canvasId);
        if (!ctx || !months || !months.length) return;

        const maxCount = Math.max(...months.map(m => m.count), 1);

        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: months.map(m => m.name),
                datasets: [{
                    label: 'Activities',
                    data: months.map(m => m.count),
                    backgroundColor: months.map(m => {
                        if (m.count === 0) return 'rgba(16,185,129,0.08)';
                        const intensity = 0.25 + (m.count / maxCount) * 0.75;
                        return `rgba(16,185,129,${intensity.toFixed(2)})`;
                    }),
                    borderColor: MINT,
                    borderWidth: 0,
                    borderRadius: 5,
                    borderSkipped: false,
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { label: ctx => ` ${ctx.parsed.y} activities` } }
                },
                scales: {
                    x: { grid: { display: false }, border: { display: false } },
                    y: {
                        grid: { color: '#EDE8E3' },
                        border: { display: false, dash: [4, 4] },
                        ticks: { stepSize: 1 },
                        beginAtZero: true,
                    }
                }
            }
        });
    };

    /* ── Mobile sidebar toggle ─────────────────────────────── */
    document.addEventListener('DOMContentLoaded', function () {
        const toggle  = document.getElementById('sidebarToggle');
        const sidebar = document.getElementById('appSidebar');
        const overlay = document.getElementById('sidebarOverlay');

        if (toggle && sidebar && overlay) {
            toggle.addEventListener('click', () => {
                sidebar.classList.toggle('is-open');
                overlay.classList.toggle('is-open');
            });
            overlay.addEventListener('click', () => {
                sidebar.classList.remove('is-open');
                overlay.classList.remove('is-open');
            });
        }
    });

})();
