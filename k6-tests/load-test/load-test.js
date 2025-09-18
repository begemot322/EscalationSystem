import http from 'k6/http';
import { check, sleep } from 'k6';
import { randomIntBetween } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';

export const options = {
    stages: [
        { duration: '30s', target: 10000 },
        { duration: '1m', target: 20000 },
    ],
    thresholds: {
        http_req_failed: ['rate<0.3'],
        http_req_duration: ['p(95)<5000'],
    },
};

export default function () {
    const featuredRes = http.get(`http://escalation.api:8080/api/escalation/featured`);

    check(featuredRes, {
        'Status is 200': (r) => r.status === 200,
        'Response time < 2s': (r) => r.timings.duration < 3000,
        'Has JSON response': (r) => {
            try {
                const json = r.json();
                return json !== null && json !== undefined;
            } catch (e) {
                return false;
            }
        }
    });

    sleep(randomIntBetween(0.1, 0.5));
}