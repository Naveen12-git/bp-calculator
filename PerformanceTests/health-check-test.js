import http from 'k6/http';
import { check } from 'k6';

export const options = {
  vus: 10,          // 10 virtual users
  duration: '1m',   // 1 minute test
  thresholds: {
    http_req_duration: ['p(95)<500'],  // 95% requests under 500ms
    http_req_failed: ['rate<0.01'],    // Error rate under 1%
  },
};

export default function () {
  const response = http.get('http://localhost:8080/health');
  
  check(response, {
    'status is 200': (r) => r.status === 200,
    'response contains Healthy': (r) => r.body.includes('Healthy'),
    'response time < 200ms': (r) => r.timings.duration < 200,
  });
}