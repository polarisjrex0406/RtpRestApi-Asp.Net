import { useLayoutEffect } from 'react';
import { useDispatch } from 'react-redux';

import { crud } from '@/redux/crud/actions';
import TestResultLayout from '@/layout/TestResultLayout/index'
import DeleteModal from '@/components/DeleteModal';
import TestResultCollapseBox from '@/components/TestResultCollapseBox';

function TestResultModule({ config, createForm, updateForm, withUpload = false }) {
  const dispatch = useDispatch();

  useLayoutEffect(() => {
    dispatch(crud.resetState());
  }, []);

  return (
    <TestResultLayout config={config}>
      <TestResultCollapseBox config={config} />
      <DeleteModal config={config} />
    </TestResultLayout>
  );
}

export default TestResultModule;
