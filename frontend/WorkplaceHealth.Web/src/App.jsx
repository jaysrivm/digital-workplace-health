import { useEffect, useState } from 'react'
import './App.css'

const API_BASE_URL = 'http://localhost:5075'

function App() {
  const [devices, setDevices] = useState([])
  const [selectedDevice, setSelectedDevice] = useState(null)
  const [history, setHistory] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  async function loadDevices() {
    try {
      setError('')

      const response = await fetch(`${API_BASE_URL}/api/devices`)

      if (!response.ok) {
        throw new Error(`API returned ${response.status}`)
      }

      const data = await response.json()

      setDevices(data)

      if (data.length > 0) {
        setSelectedDevice(data[0])
      }
    } catch (err) {
      setError(
        'Unable to connect to the Workplace Health API. Make sure the backend is running.'
      )
      console.error(err)
    } finally {
      setLoading(false)
    }
  }

  async function loadHistory(deviceId) {
    try {
      const response = await fetch(
        `${API_BASE_URL}/api/devices/${deviceId}/history`
      )

      if (!response.ok) {
        throw new Error(`API returned ${response.status}`)
      }

      const data = await response.json()

      setHistory(Array.isArray(data) ? data : [data])
    } catch (err) {
      console.error(err)
      setHistory([])
    }
  }

  useEffect(() => {
    loadDevices()
  }, [])

  useEffect(() => {
    if (selectedDevice) {
      loadHistory(selectedDevice.id)
    }
  }, [selectedDevice])

  const latestReport = history.length > 0 ? history[0] : null

  const cpuUsage = latestReport?.cpu?.cpuUsagePercent ?? 0
  const memoryUsage = latestReport?.memory?.memoryUsagePercent ?? 0

  const getDiskUsage = () => {
    const disks = latestReport?.disks ?? []

    if (disks.length === 0) {
      return 0
    }

    return Math.round(
      disks.reduce(
        (total, disk) => total + (100 - disk.freePercent),
        0
      ) / disks.length
    )
  }

  const diskUsage = getDiskUsage()

  const calculateHealthScore = () => {
    if (!latestReport) {
      return {
        score: 0,
        label: 'No data',
        className: 'status-neutral',
        issues: [],
      }
    }

    let score = 100
    const issues = []

    // CPU
    if (cpuUsage >= 90) {
      score -= 25
      issues.push('CPU usage is very high')
    } else if (cpuUsage >= 75) {
      score -= 10
      issues.push('CPU usage is high')
    }

    // Memory
    if (memoryUsage >= 90) {
      score -= 25
      issues.push('Memory usage is very high')
    } else if (memoryUsage >= 75) {
      score -= 15
      issues.push('Memory usage is high')
    }

    // Disk
    if (diskUsage >= 90) {
      score -= 20
      issues.push('Disk usage is very high')
    } else if (diskUsage >= 75) {
      score -= 10
      issues.push('Disk usage is high')
    }

    // Windows Updates
    const pendingUpdates =
      latestReport.windowsUpdate?.pendingUpdateCount ?? 0

    const updateCheckSucceeded =
      latestReport.windowsUpdate?.updateCheckSucceeded ?? false

    if (!updateCheckSucceeded) {
      score -= 15
      issues.push('Windows Update check failed')
    } else if (pendingUpdates >= 5) {
      score -= 15
      issues.push(`${pendingUpdates} Windows updates are pending`)
    } else if (pendingUpdates > 0) {
      score -= 5
      issues.push(`${pendingUpdates} Windows updates are pending`)
    }

    // Services
    const services = latestReport.services ?? []

    const stoppedServices = services.filter(
      (service) =>
        service.status &&
        service.status.toLowerCase() !== 'running'
    )

    if (stoppedServices.length > 0) {
      score -= Math.min(stoppedServices.length * 5, 15)
      issues.push(
        `${stoppedServices.length} important service${
          stoppedServices.length > 1 ? 's are' : ' is'
        } not running`
      )
    }

    score = Math.max(0, Math.min(100, score))

    let label = 'Healthy'
    let className = 'status-good'

    if (score < 60) {
      label = 'Needs attention'
      className = 'status-danger'
    } else if (score < 80) {
      label = 'Warning'
      className = 'status-warning'
    }

    return {
      score,
      label,
      className,
      issues,
    }
  }

  const healthScore = calculateHealthScore()
  const healthStatus = healthScore
  return (
    <div className="app">
      <header className="topbar">
        <div>
          <p className="eyebrow">DIGITAL WORKPLACE</p>
          <h1>Workplace Health</h1>
        </div>

        <div className={`system-status ${healthStatus.className}`}>
          <span className="status-dot"></span>
          {healthStatus.label}
        </div>
      </header>

      <main className="dashboard">
        {loading && (
          <div className="message-card">
            Loading workplace health data...
          </div>
        )}

        {error && (
          <div className="message-card error-card">
            {error}
          </div>
        )}

        {!loading && !error && devices.length === 0 && (
          <div className="message-card">
            No devices have reported health data yet.
          </div>
        )}

        {!loading && devices.length > 0 && (
          <>
            <section className="device-section">
              <div className="section-heading">
                <div>
                  <p className="section-label">DEVICES</p>
                  <h2>Your devices</h2>
                </div>

                <span className="device-count">
                  {devices.length} device{devices.length !== 1 ? 's' : ''}
                </span>
              </div>

              <div className="device-list">
                {devices.map((device) => (
                  <button
                    key={device.id}
                    className={`device-card ${
                      selectedDevice?.id === device.id
                        ? 'selected'
                        : ''
                    }`}
                    onClick={() => setSelectedDevice(device)}
                  >
                    <div className="device-icon">PC</div>

                    <div className="device-info">
                      <strong>{device.deviceName}</strong>

                      <span>
                        Last seen:{' '}
                        {new Date(
                          device.lastSeenAtUtc
                        ).toLocaleString()}
                      </span>
                    </div>

                    <span className="online-badge">
                      <span className="status-dot"></span>
                      Online
                    </span>
                  </button>
                ))}
              </div>
            </section>

            {selectedDevice && (
              <section className="device-section">
                <div className="section-heading">
                  <div>
                    <p className="section-label">DEVICE OVERVIEW</p>
                    <h2>{selectedDevice.deviceName}</h2>
                  </div>

                  <span className={`health-badge ${healthStatus.className}`}>
                    <span className="status-dot"></span>
                    {healthStatus.label}
                  </span>
                </div>

                {latestReport && (
  <>
    <div className="overall-health">
      <div className="overall-health-info">
        <p className="section-label">OVERALL HEALTH</p>

        <div className="health-score-row">
          <strong>{healthScore.score}</strong>
          <span>/ 100</span>
        </div>

        <div
          className={`health-score-status ${healthScore.className}`}
        >
          <span className="status-dot"></span>
          {healthScore.label}
        </div>

        {healthScore.issues.length > 0 && (
          <div className="health-issues">
            {healthScore.issues.map((issue) => (
              <div key={issue} className="health-issue">
                <span>•</span>
                {issue}
              </div>
            ))}
          </div>
        )}

        {healthScore.issues.length === 0 && (
          <p className="health-good-message">
            No significant issues detected.
          </p>
        )}
      </div>

      <div className="health-score-circle">
        <div className="health-score-circle-inner">
          <strong>{healthScore.score}</strong>
          <span>Health</span>
        </div>
      </div>
    </div>

    <div className="metric-grid">
      <MetricCard
        title="CPU"
        value={`${cpuUsage}%`}
        description={
          latestReport.cpu?.cpuName ||
          'Processor'
        }
        percentage={cpuUsage}
      />

      <MetricCard
        title="Memory"
        value={`${Math.round(memoryUsage)}%`}
        description={`${latestReport.memory?.availableMemoryGb?.toFixed(
          1
        )} GB available`}
        percentage={memoryUsage}
      />

      <MetricCard
        title="Disk"
        value={`${diskUsage}%`}
        description="Average disk usage"
        percentage={diskUsage}
      />

      <MetricCard
        title="Windows"
        value={
          latestReport.registry
            ?.windowsDisplayVersion || 'Unknown'
        }
        description={
          latestReport.registry
            ?.windowsProductName || ''
        }
        percentage={null}
      />
    </div>
                    <div className="two-column">
                      <section className="panel">
                        <div className="panel-heading">
                          <div>
                            <p className="section-label">
                              WINDOWS UPDATE
                            </p>
                            <h3>Update status</h3>
                          </div>

                          <span className="status-good">
                            {latestReport.windowsUpdate
                              ?.windowsUpdateServiceStatus ||
                              'Unknown'}
                          </span>
                        </div>

                        <div className="update-details">
                          <div>
                            <span>Pending updates</span>
                            <strong>
                              {latestReport.windowsUpdate
                                ?.pendingUpdateCount ?? 0}
                            </strong>
                          </div>

                          <div>
                            <span>Update check</span>
                            <strong>
                              {latestReport.windowsUpdate
                                ?.updateCheckSucceeded
                                ? 'Succeeded'
                                : 'Failed'}
                            </strong>
                          </div>
                        </div>
                      </section>

                      <section className="panel">
                        <div className="panel-heading">
                          <div>
                            <p className="section-label">
                              SYSTEM SERVICES
                            </p>
                            <h3>Important services</h3>
                          </div>
                        </div>

                        <div className="service-list">
                          {(latestReport.services ?? []).map(
                            (service) => (
                              <div
                                className="service-row"
                                key={service.name}
                              >
                                <span>
                                  <span className="status-dot"></span>
                                  {service.displayName}
                                </span>

                                <strong>{service.status}</strong>
                              </div>
                            )
                          )}
                        </div>
                      </section>
                    </div>

                    <section className="panel history-panel">
                      <div className="panel-heading">
                        <div>
                          <p className="section-label">
                            HEALTH HISTORY
                          </p>
                          <h3>Recent reports</h3>
                        </div>

                        <span>
                          {history.length} report
                          {history.length !== 1 ? 's' : ''}
                        </span>
                      </div>

                      <div className="history-table">
                        <div className="history-row history-header">
                          <span>Time</span>
                          <span>CPU</span>
                          <span>Memory</span>
                          <span>Pending Updates</span>
                        </div>

                        {history
                          .slice()
                          .reverse()
                          .slice(0, 10)
                          .map((report) => (
                            <div
                              className="history-row"
                              key={report.id}
                            >
                              <span>
                                {new Date(
                                  report.collectedAtUtc
                                ).toLocaleTimeString()}
                              </span>

                              <span>
                                {report.cpu?.cpuUsagePercent}%
                              </span>

                              <span>
                                {Math.round(
                                  report.memory
                                    ?.memoryUsagePercent ?? 0
                                )}
                                %
                              </span>

                              <span>
                                {report.windowsUpdate
                                  ?.pendingUpdateCount ?? 0}
                              </span>
                            </div>
                          ))}
                      </div>
                    </section>
                  </>
                )}
              </section>
            )}
          </>
        )}
      </main>
    </div>
  )
}

function MetricCard({
  title,
  value,
  description,
  percentage,
}) {
  return (
    <div className="metric-card">
      <div className="metric-top">
        <span>{title}</span>

        {percentage !== null && (
          <span className="metric-percent">
            {Math.round(percentage)}%
          </span>
        )}
      </div>

      <strong className="metric-value">{value}</strong>

      <span className="metric-description">
        {description}
      </span>

      {percentage !== null && (
        <div className="progress-track">
          <div
            className="progress-bar"
            style={{
              width: `${Math.min(
                Math.max(percentage, 0),
                100
              )}%`,
            }}
          ></div>
        </div>
      )}
    </div>
  )
}

export default App