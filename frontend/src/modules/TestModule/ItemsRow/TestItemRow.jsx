import { Form, Row, Col } from 'antd';
import { DeleteOutlined } from '@ant-design/icons';

import AutoCompleteAsync from '@/components/AutoCompleteAsync';

export default function TestItemRow({ field, remove, current = null, curTopicId }) {
  return (
    <Row gutter={[12, 12]} style={{ position: 'relative' }}>
      <Col className="gutter-row" span={24}>
        <Form.Item
          name={[field.name, 'experiment']}
          rules={[
            {
              required: true,
            },
          ]}
        >
          <AutoCompleteAsync
            entity={'experiment'}
            displayLabels={['experimentCode']}
            searchFields={'experimentCode'}
            redirectLabel={'Add New Experiment'}
            withRedirect
            urlToRedirect={'/experiment/create'}
            curTopicId={curTopicId}
          />
        </Form.Item>
      </Col>
      <div style={{ position: 'absolute', right: '-20px', top: ' 5px' }}>
        <DeleteOutlined onClick={() => remove(field.name)} />
      </div>
    </Row>
  );
}
