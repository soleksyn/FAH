// AI Dashboard - AJAX handlers for AI analysis buttons
document.addEventListener('DOMContentLoaded', function() {
    const aiPanel = document.getElementById('aiPanel');
    const aiLoader = document.getElementById('aiLoader');
    const aiButtons = document.querySelectorAll('.ai-action-btn');

    if (!aiPanel || !aiLoader) return;

    // Show/hide loader
    function showLoader() {
        aiLoader.classList.add('loading');
        aiPanel.classList.add('loading');
    }

    function hideLoader() {
        aiLoader.classList.remove('loading');
        aiPanel.classList.remove('loading');
    }

    // Render AI analysis in the panel
    function renderAnalysis(analysis) {
        const aiResults = document.getElementById('aiResults');
        if (!aiResults) return;
        
        let html = '';
        
        if (analysis.analysis) {
            html += `
                <div class="insight-block">
                    <div class="insight-block-title"><i class="bi bi-graph-up-arrow"></i> Performance Analysis</div>
                    <p class="analysis-paragraph">${escapeHtml(analysis.analysis)}</p>
                </div>
            `;
        }

        if (analysis.keyInsights && analysis.keyInsights.length > 0) {
            html += `
                <div class="insight-block">
                    <div class="insight-block-title"><i class="bi bi-lightbulb"></i> Key Insights</div>
                    <ul class="insight-list">
                        ${analysis.keyInsights.map(insight => `<li>${escapeHtml(insight)}</li>`).join('')}
                    </ul>
                </div>
            `;
        }

        if (analysis.recommendations && analysis.recommendations.length > 0) {
            html += `
                <div class="insight-block">
                    <div class="insight-block-title"><i class="bi bi-check2-circle"></i> Recommendations</div>
                    <ul class="insight-list">
                        ${analysis.recommendations.map(rec => `<li>${escapeHtml(rec)}</li>`).join('')}
                    </ul>
                </div>
            `;
        }

        // Add quick actions
        const athleteId = aiPanel.dataset.athleteId;
        if (athleteId) {
            html += `
                <div class="quick-actions">
                    <button type="button" class="quick-action-btn ai-action-btn"
                            data-action="GetPerformanceTrends">
                        <i class="bi bi-graph-up"></i> Performance Trends
                    </button>
                    <button type="button" class="quick-action-btn ai-action-btn"
                            data-action="GetTrainingRecommendations">
                        <i class="bi bi-lightbulb"></i> Training Tips
                    </button>
                    <button type="button" class="quick-action-btn ai-action-btn"
                            data-action="GetHealthMetrics">
                        <i class="bi bi-heart-pulse"></i> Health Analysis
                    </button>
                    <button type="button" class="quick-action-btn ai-action-btn"
                            data-action="GetNextDayRecommendation">
                        <i class="bi bi-calendar-check"></i> Plan Next Workout
                    </button>
                </div>
            `;
        }

        aiResults.innerHTML = html;
        
        // Re-attach event listeners to new buttons
        attachButtonListeners();
    }

    // Escape HTML to prevent XSS
    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Handle button click
    async function handleButtonClick(button) {
        const action = button.dataset.action;
        const activityId = button.dataset.activityId;
        const athleteId = aiPanel.dataset.athleteId;

        if (!athleteId) {
            alert('Athlete ID not found');
            return;
        }

        showLoader();

        // Determine endpoint and data
        let endpoint = '';
        let data = { id: athleteId };

        switch (action) {

            case 'GetPerformanceTrends':
                endpoint = '/Dashboard/GetPerformanceTrendsJson';
                break;
            case 'GetTrainingRecommendations':
                endpoint = '/Dashboard/GetTrainingRecommendationsJson';
                break;
            case 'GetHealthMetrics':
                endpoint = '/Dashboard/GetHealthMetricsJson';
                break;
            case 'GetNextDayRecommendation':
                endpoint = '/Dashboard/GetNextDayRecommendationJson';
                break;
            default:
                hideLoader();
                alert('Unknown action');
                return;
        }

        try {
            const response = await fetch(endpoint, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                },
                body: new URLSearchParams(data)
            });

            const result = await response.json();

            if (result.success) {
                renderAnalysis(result.data);
            } else {
                alert('Error: ' + (result.error || 'AI analysis failed'));
            }
        } catch (error) {
            alert('Error: ' + error.message);
        } finally {
            hideLoader();
        }
    }

    // Attach event listeners to AI buttons
    function attachButtonListeners() {
        const buttons = aiPanel.querySelectorAll('.ai-action-btn');
        buttons.forEach(button => {
            button.removeEventListener('click', handleButtonClick);
            button.addEventListener('click', function(e) {
                e.preventDefault();
                handleButtonClick(button);
            });
        });
    }

    // Initial attachment
    attachButtonListeners();
});
